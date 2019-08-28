using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AWI.HierarchyWindowExtension {

	 public static class ControllerStyles {

		  public static GUIStyle lazy_LayerLabel = null;

		  public static GUIStyle layerLabel {
				get {
					 if (lazy_LayerLabel == null) {
						  lazy_LayerLabel = new GUIStyle(EditorStyles.label);
						  lazy_LayerLabel.alignment = TextAnchor.MiddleRight;
						  lazy_LayerLabel.fontStyle = FontStyle.Bold;
						  lazy_LayerLabel.fontSize = 8;
					 }
					 return lazy_LayerLabel;
				}
		  }

		  private static GUIStyle lazy_Button = null;

		  public static GUIStyle button {
				get {
					 if (lazy_Button == null) {
						  lazy_Button = new GUIStyle(EditorStyles.label);
						  lazy_Button = new GUIStyle(EditorStyles.miniButton);
						  lazy_Button.border = new RectOffset(0, 0, 0, 0);
						  lazy_Button.padding = new RectOffset(0, 0, 0, 0);
						  lazy_Button.fontSize = 8;
						  lazy_Button.fontStyle = FontStyle.Normal;
					 }
					 return lazy_Button;
				}
		  }

		  public static GUIContent openCloseTool = new GUIContent("D", "activate deactivate / open close");

		  public static GUIContent tools = new GUIContent("T", "tools");

		  public static GUIContent setActiveToggle = new GUIContent("", "active / deactive");

		  public static GUIContent[] siblingTools = { new GUIContent(SpecChars.down, "sibling down"), new GUIContent(SpecChars.up, "sibling up") };

	 }

	 [InitializeOnLoad]
	 public class Controller {

		  private static bool openClose = false;
		  private static bool kOffsetDirt = false;
		  private static int kOffset = 0;
		  private const int leftSpace = 30;

		  private static List<Action> siblingActions;

		  private static void SetOffsetDirt() {
				kOffsetDirt = true;
		  }

		  static Controller() { 
				EditorApplication.hierarchyChanged -= SetOffsetDirt;
				EditorApplication.hierarchyChanged += SetOffsetDirt;
				EditorApplication.hierarchyWindowItemOnGUI -= HierarchyItemCB;
				EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
		  }

		  private static void CheckOffsetDirt(int instanceID, Rect selectionRect) {
				if (kOffsetDirt) {
					 kOffset = (int)selectionRect.y - (((int)selectionRect.y / (int)selectionRect.height)) * (int)selectionRect.height;
					 kOffsetDirt = false;
				}
		  }

		  /// <summary>
		  /// [0] background color [1] text color
		  /// </summary>
		  private static Color[] layerGuiColor = new Color[2];
		  private static string layerNameCache = "";
		  private static int lastLayerDrawed = -1000;

		  private static Color activeColor = new Color(1, 1, 1, 1);
		  private static Color nonActiveColor = new Color(1, 1, 1, 0.5f);
		  private static Color selectionHilightColor = new Color(0, 0, 0, 0.15f);
		  private static Rect selectionHilight = new Rect(0, 0, 0, 15);
		  private static Rect toggleRect = new Rect(0, 0, 15, 15);
		  private static Rect layerRect = new Rect(0, 0, 15, 15);

		  private static bool isOpenCloseRegistred = false;
		  private static Type openCloseType = null;
		  private static Action<System.Object> openMethod;
		  private static Action<System.Object> closeMethod;
		  private static Func<System.Object, bool> isOpenFunc;

		  public static void RegistrOpenCloseInfo(Type type, Action<System.Object> openMethod, Action<System.Object> closeMethod, Func<System.Object, bool> isOpenFunc) {
				Controller.openCloseType = type;
				Controller.openMethod = openMethod;
				Controller.closeMethod = closeMethod;
				Controller.isOpenFunc = isOpenFunc;
				isOpenCloseRegistred = true;
		  }

		  private static void HierarchyItemCB(int instanceID, Rect selectionRect) {
				CheckOffsetDirt(instanceID, selectionRect);

				// copy y selection layout
				selectionHilight.height = selectionRect.height;
				selectionHilight.y = selectionRect.y;
				// left space
				selectionHilight.width = selectionRect.x;
				selectionHilight.x = 0;
				
				//EditorGUI.DrawRect(selectionHilight, selectionHilightColor);
	 			
				bool doToolMenu = false;

				var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

				if (gameObject == null) {
					 var clipRect = new Rect(selectionRect);
					 clipRect.width -= 20;
					 GUI.BeginClip(clipRect);
					 var localTopBar = new Rect(selectionRect.xMax - 100, selectionRect.y, 100, selectionRect.height);
					 var popupRect = localTopBar.CopyWidth(20);
					 doToolMenu = GUI.Button(popupRect, ControllerStyles.tools);
					 if (isOpenCloseRegistred) {
						  var openCloseRect = popupRect.NextRight().CopyWidth(30);
						  openCloseRect.x += 5;
						  openClose = GUI.Toggle(openCloseRect, openClose, ControllerStyles.openCloseTool);
					 }
					 GUI.EndClip();
				} else {
					 toggleRect.x = selectionRect.xMax - 15 + 3;

					 layerRect.y = toggleRect.y = selectionRect.y;
					 layerRect.width = selectionRect.width - 15;
					 layerRect.x = selectionRect.x;
					 layerRect.height = selectionRect.height;

					 //parity
					 if (((int)selectionRect.y - kOffset) / ((int)selectionRect.height) % 2 == 0) {
						  EditorGUI.DrawRect(selectionRect, new Color(1, selectionRect.y / 255.0f, 1, 0.015f));
					 }

					 using (new GUIAddons.ColorScope(gameObject.activeInHierarchy ? activeColor : nonActiveColor)) {
						  bool active = false;
						  bool drawNormalToggle = true;
						  if (isOpenCloseRegistred && openClose) {
								var iOpenClose = gameObject.GetComponent(openCloseType);
								if(iOpenClose != null) {
									 var isOpen = isOpenFunc(iOpenClose);
									 using (new GUIAddons.ColorBackgroundScope(Color.cyan)) {
										  active = GUI.Toggle(toggleRect, isOpen, GUIContent.none, MiscStyles.shurikenToggle);
										  if(active != isOpen) {
												if (active) {
													 openMethod(iOpenClose);
												}else {
													 closeMethod(iOpenClose);
												}
										  }
									 }
									 drawNormalToggle = false;
								}
						  }
						  if (drawNormalToggle) {
								using (new GUIAddons.ColorBackgroundScope(Color.white)) {
									 active = GUI.Toggle(toggleRect, gameObject.activeSelf, GUIContent.none, MiscStyles.shurikenToggle);
								}
								if (active != gameObject.activeSelf) {
									 gameObject.SetActive(active);
								}
						  }

						  // draw layer layer	
						  bool firstLayer = gameObject.transform.parent == null || gameObject.transform.parent.gameObject.layer != gameObject.layer;

						  if(lastLayerDrawed != gameObject.layer) {
								lastLayerDrawed = gameObject.layer;
								layerNameCache = LayerMask.LayerToName(gameObject.layer);
								ColorPallete.LayerStyle(gameObject.layer, layerNameCache, ref layerGuiColor[0], ref layerGuiColor[1]);
						  }

						  if (!firstLayer) {
								layerRect.x = layerRect.xMax - 1;
								layerRect.width = 1;
						  } else {
								float width = ControllerStyles.layerLabel.CalcSize(new GUIContent(layerNameCache)).x;
								layerRect.y += 2;
								layerRect.height -= 2;
								layerRect.x = layerRect.xMax - width;
								layerRect.width = width;
						  }
						  var layerRectCopy = new Rect(layerRect);
						  layerRectCopy.x -= 1;
						  layerRectCopy.width += 2;
						  EditorGUI.DrawRect(layerRectCopy, layerGuiColor[0]);
						  if (firstLayer) {
								using (new GUIAddons.ColorScope(layerGuiColor[1])) {
									 GUI.Label(layerRect, layerNameCache, ControllerStyles.layerLabel);
								}
						  }
						 
					 }
				}

				if (doToolMenu) {
					 HierarchyWindowExtensionTools.ShowMenu();
				}
		  }

		  [InitializeOnLoad]
		  protected static class HierarchyWindowExtensionTools {
				
				public static void ShowMenu() {
					 var genericMenu = new GenericMenu();
					 genericMenu.AddItem(new GUIContent("Names/Clean Up Names"), false, CleanUpName);
					 genericMenu.AddItem(new GUIContent("Names/Clean Up Names (Include Children)"), false, CleanUpChildrenNames);
					 genericMenu.AddSeparator("");
					 genericMenu.AddItem(new GUIContent("Copy Transform"), false, CopyTransform);
					 genericMenu.AddItem(new GUIContent("Paste Transform"), false, PasteTransform);
					 genericMenu.AddSeparator("");
					 genericMenu.AddItem(new GUIContent("Reset Transform"), false, ResetTransform);
					 genericMenu.AddItem(new GUIContent("Reset Position"), false, ResetPosition);
					 genericMenu.AddItem(new GUIContent("Reset Rotation"), false, ResetRotation);
					 genericMenu.AddItem(new GUIContent("Reset Scale"), false, ResetScale);
					 genericMenu.AddSeparator("");
					 genericMenu.AddItem(new GUIContent("Call Garbage Collector    (╯°□°）╯︵ ┻━┻"), false, CallGarbageCollector);
					 genericMenu.ShowAsContext();
				}
								
				private static Transform transformClippboard;

				public static void CallGarbageCollector() {
					 GC.Collect();
				}

				public static void CopyTransform() {
					 if (Selection.activeGameObject != null) {
						  transformClippboard = Selection.activeGameObject.transform;
					 }
				}

				public static void PasteTransform() {
					 if (transformClippboard == null) {
						  return;
					 }
					 UnityEditorInternal.ComponentUtility.CopyComponent(transformClippboard);
					 foreach (var go in Selection.gameObjects) {
						  var transform = go.transform;
						  UnityEditorInternal.ComponentUtility.PasteComponentValues(transform);
					 }
				}

				public static void ResetTransform() {
					 foreach (var go in Selection.gameObjects) {
						  var rectTransform = go.transform as RectTransform;
						  if (rectTransform != null) {
								rectTransform.anchoredPosition = Vector3.zero;
						  } else {
								go.transform.localPosition = Vector3.zero;
						  }
						  go.transform.localEulerAngles = Vector3.zero;
						  go.transform.localScale = Vector3.one;
					 }
				}

				public static void ResetPosition() {
					 foreach (var go in Selection.gameObjects) {
						  var rectTransform = go.transform as RectTransform;
						  if (rectTransform != null) {
								rectTransform.anchoredPosition = Vector3.zero;
						  } else {
								go.transform.localPosition = Vector3.zero;
						  }
					 }
				}

				public static void ResetRotation() {
					 foreach (var go in Selection.gameObjects) {
						  go.transform.eulerAngles = Vector3.zero;
					 }
				}

				public static void ResetScale() {
					 foreach (var go in Selection.gameObjects) {
						  go.transform.localScale = Vector3.one;
					 }
				}

				public static void CleanUpName() {
					 foreach (var go in Selection.gameObjects) {
						  go.name = StringUtilities.CleanCopiedName(go.name);
					 }
				}

				public static void CleanUpChildrenNames() {
					 foreach (var go in Selection.gameObjects) {
						  CleanUpChildrenNames(go.transform);
					 }
				}

				public static void CleanUpChildrenNames(Transform transform) {
					 transform.gameObject.name = StringUtilities.CleanCopiedName(transform.gameObject.name);
					 foreach (Transform v in transform) {
						  CleanUpChildrenNames(v);
					 }
				}

				private static List<Selectable> FindSelectableChilds(Transform parent) {
					 var selectableNeighbours = new List<Selectable>();
					 int parentChildCount = parent.childCount;

					 for (int i = 0; i < parentChildCount; ++i) {
						  var selectableNeighbour = parent.GetChild(i).GetComponent<Selectable>();
						  if (selectableNeighbour != null) {
								selectableNeighbours.Add(selectableNeighbour);
						  }
					 }
					 return selectableNeighbours;
				}
				
				private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir) {
					 if (rect == null)
						  return Vector3.zero;
					 if (dir != Vector2.zero)
						  dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
					 dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
					 return dir;
				}

				public static Selectable FindSelectable(List<Selectable> list, Selectable selectable, Vector3 dir) {
					 Transform transform = selectable.transform;
					 dir = dir.normalized;
					 Vector3 localDir = Quaternion.Inverse(transform.rotation) * dir;
					 Vector3 pos = transform.TransformPoint(GetPointOnRectEdge(transform as RectTransform, localDir));
					 float maxScore = Mathf.NegativeInfinity;
					 Selectable bestPick = null;
					 for (int i = 0; i < list.Count; ++i) {
						  Selectable sel = list[i];

						  if (sel == selectable || sel == null)
								continue;

						  if (sel.navigation.mode == Navigation.Mode.None)
								continue;

						  var selRect = sel.transform as RectTransform;
						  Vector3 selCenter = selRect != null ? (Vector3)selRect.rect.center : Vector3.zero;
						  Vector3 myVector = sel.transform.TransformPoint(selCenter) - pos;
						  
						  float dot = Vector3.Dot(dir, myVector);

						  if (dot <= 0)
								continue;
						  float score = dot / myVector.sqrMagnitude;

						  if (score > maxScore) {
								maxScore = score;
								bestPick = sel;
						  }
					 }
					 return bestPick;
				}

		  }


	 }

}