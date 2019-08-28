using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AWI;

namespace UMa {

	 public static class UMaEditorUtilities {
		  private static Material mat;

		  public static void DrawTransformPreview(Rect rect, Vector3 direction, Vector3 rotation, Vector3 scale) {
				var center = new Vector3(rect.center.x, rect.center.y, 0);
				if (true) {
					 var d = Quaternion.Euler(rotation) * Vector3.right;
					 var b = 0.8f * d * rect.width / 2;
					 Handles.color = Color.blue;
					 Handles.DrawBezier(center, center + b, center + d / 10, center + b - d / 10, Color.blue, null, 4);
					 Handles.color = Color.white;
					 var capPos = center + b;
					 capPos.z = -100;
					 var capRot = Quaternion.LookRotation(-b);
					 Handles.color = new Color(1, 0, 0, 0.5f);
					 Handles.ConeHandleCap(0, capPos, capRot, 8, EventType.Repaint);
					 Handles.ConeHandleCap(0, capPos, capRot, 9, EventType.Repaint);
					 Handles.ConeHandleCap(0, capPos, capRot, 10, EventType.Repaint);
					 Handles.color = Color.white;
				}
				if (direction.magnitude > 0) {
					 var d = -direction.normalized;
					 var b = 0.5f * d * rect.width / 2;
					 Handles.DrawBezier(center, center + b, center + d / 10, center + b - d / 10, Color.black, null, 2);
					 var capPos = center + b;
					 capPos.z = -100;
					 var capRot = Quaternion.LookRotation(-b);
					 Handles.color = Color.black * 0.3333f;
					 Handles.ConeHandleCap(0, capPos, capRot, 8, EventType.Repaint);
					 Handles.ConeHandleCap(0, capPos, capRot, 9, EventType.Repaint);
					 Handles.ConeHandleCap(0, capPos, capRot, 10, EventType.Repaint);
					 Handles.color = Color.white;
				}

		  }

		  public static void DrawConnection(Vector2 a, Vector2 b, bool spring, Color color) {
				var l = Vector2.Distance(a, b);
				var c = a + Vector2.right * l / 8;
				var d = b + Vector2.left * l / 8;

				var capPos = new Vector3(0, 0, -100);
				var capRot = Quaternion.identity;
				bool drawCap = true;

				if (spring) {
					 var steps = (int)(1 + l / 10);
					 var p0 = a;
					 var dp = (b - a) / steps;

					 var t4_a = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 45)).MultiplyVector(new Vector2(dp.x, dp.y));
					 var t2_a = new Vector2(t4_a.x, t4_a.y);
					 var t4_b = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -45)).MultiplyVector(new Vector2(-dp.x, -dp.y));
					 var t2_b = new Vector2(-t4_b.x, -t4_b.y);

					 t2_a = t2_a.normalized * 5;
					 t2_b = t2_b.normalized * 5;

					 for (int i = 0; i < steps; ++i) {
						  Handles.DrawBezier(p0, p0 + dp, p0 + t2_a, p0 + dp - t2_b, color, null, 3);
						  p0 += dp;
						  if (i == steps / 2) {
								capPos.x = p0.x;
								capPos.y = p0.y;
						  }
					 }
					 capRot = Quaternion.LookRotation(b - a);
					 drawCap = false;
				} else {
					 Handles.DrawBezier(a, b, c, d, color * 0.95f, null, 2);
					 var points = Handles.MakeBezierPoints(a, b, c, d, 7);
					 capPos.x = points[3].x;
					 capPos.y = points[3].y;
					 capRot = Quaternion.LookRotation(points[3] - points[2]);
				}

				if (drawCap) {
					 Handles.ConeHandleCap(0, capPos, capRot, 9, EventType.Repaint);
				}
		  }

		  public static void DrawGrid(Rect position) {
				if (Event.current.type == EventType.Repaint) {
					 Handles.BeginGUI();
					 if (!mat) {
						  mat = new Material(Shader.Find("Unlit/Color"));
					 }
					 mat.color = new Color32(46, 46, 46, 255).SpaceSuit();
					 mat.SetPass(0);

					 GL.Begin(GL.TRIANGLE_STRIP);
					 GL.Vertex(new Vector2(position.x, position.y));
					 GL.Vertex(new Vector2(position.x, position.yMax));
					 GL.Vertex(new Vector2(position.xMax, position.yMax));
					 GL.Vertex(new Vector2(position.xMax, position.y));
					 GL.Vertex(new Vector2(position.x, position.y));
					 GL.Vertex(new Vector2(position.x, position.yMax));
					 GL.End();

					 mat.color = new Color32(39, 39, 39, 255).SpaceSuit();
					 mat.SetPass(0);

					 position = new RectOffset(-50, -50, -50, -50).Add(position);

					 GL.Begin(GL.LINES);
					 for (float i = position.y; i < position.yMax; i += 64) {
						  GL.Vertex(new Vector2(position.x, i));
						  GL.Vertex(new Vector2(position.xMax, i));
					 }
					 GL.Vertex(new Vector2(position.x, position.yMax));
					 GL.Vertex(new Vector2(position.xMax, position.yMax));
					 for (float i = position.x; i < position.xMax; i += 64) {
						  GL.Vertex(new Vector2(i, position.y));
						  GL.Vertex(new Vector2(i, position.yMax));
					 }
					 GL.Vertex(new Vector2(position.xMax, position.y));
					 GL.Vertex(new Vector2(position.xMax, position.yMax));
					 GL.End();

					 Handles.EndGUI();
				}
		  }

		  public static int DoHorizontalToolbar(int selectedOption, GUIContent[] content, GUIContent[] pressedContent, int width, int height) {
				var rect = EditorGUILayout.GetControlRect(GUILayout.Width(width), GUILayout.Height(height));
				rect.width /= content.Length;
				rect.width -= 1;
				GUILayout.BeginHorizontal();
				for (int i = 0; i < content.Length; ++i) {
					 var style = MiscStyles.ToolbarButton(i, content.Length);
					 var t = GUI.Toggle(rect, i == selectedOption, i == selectedOption ? pressedContent[i] : content[i], style);
					 if (t == true) {
						  selectedOption = i;
					 }
					 rect.x = rect.xMax;
				}
				GUILayout.EndHorizontal();
				return selectedOption;
		  }

		  public static int DoVerticalToolbar(int selectedOption, GUIContent[] content, GUIContent[] pressedContent, Rect rect) {
				rect.height = 24;
				var style = MiscStyles.rightButton;
				for (int i = 0; i < content.Length; ++i) {
					 var t = GUI.Toggle(rect, i == selectedOption, i == selectedOption ? pressedContent[i] : content[i], style);
					 if (t == true) {
						  selectedOption = i;
					 }
					 rect.y = rect.yMax;
				}
				return selectedOption;
		  }

		  public static void DrawLine(Vector2 p1, Vector2 p2) {
				GL.Vertex(p1);
				GL.Vertex(p2);
		  }

	 }

}
