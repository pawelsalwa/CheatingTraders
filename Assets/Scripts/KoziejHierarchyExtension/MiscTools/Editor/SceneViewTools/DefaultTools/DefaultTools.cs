using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace AWI.SceneViewTools {
	 public static class DefaultCemeraTools {
		  static private GUIContent focusCameraContent;
		  static private GUIContent followCameraContent;

		  static private bool trackCamera = false;
		  static private Quaternion cameraRotationCache = Quaternion.identity;
		  static private Vector3 cameraPositionCache = Vector3.zero;

		  static public void Init() {
				focusCameraContent = new GUIContent(IconTextureCache.Get("FocusCamera"), "focus main camera");
				followCameraContent = new GUIContent(IconTextureCache.Get("FollowCamera"), "slave main camera");
		  }

		  static public void Draw() {
				var focusCamera = Layout.Button(focusCameraContent);
				var trackCamera = Layout.Toggle(followCameraContent, DefaultCemeraTools.trackCamera);

				if (DefaultCemeraTools.trackCamera != trackCamera) {
					 DefaultCemeraTools.trackCamera = trackCamera;
					 if (Camera.main == null) {
						  DefaultCemeraTools.trackCamera = trackCamera = false;
					 } else {
						  if (trackCamera) {
								cameraPositionCache = Camera.main.transform.position;
								cameraRotationCache = Camera.main.transform.rotation;
						  } else {
								Camera.main.transform.position = cameraPositionCache;
								Camera.main.transform.rotation = cameraRotationCache;
						  }
					 }
				}

				if (trackCamera) {
					 if (SceneView.currentDrawingSceneView != null) {
						  Camera.main.transform.position = SceneView.currentDrawingSceneView.camera.transform.position;
						  Camera.main.transform.rotation = SceneView.currentDrawingSceneView.camera.transform.rotation;
					 }
				}


				if (focusCamera) {
					 var mainCamera = Camera.main;
					 if (mainCamera != null) {
						  SceneView.lastActiveSceneView.AlignViewToObject(mainCamera.transform);
					 }
				}
		  }
	 };

	 public static class DefaultOverlayCameraTools {
		  private const float phi = 1.6180339887f;
		  enum Options { GoldenRatio, Diagonal, Thirds, Ps4GUI, Spiral, Dragon, Triangle };
		  enum Spirals { LeftTop, RightDown, TopRight, DownLeft };
		  enum Triangles { Golden, Harmonious }

		  private static Material lineMaterial;

		  private static int dragonIT = 12;

		  private static GUIContent overlayCameraContent;

		  private static bool overlayOnlyOnMainCamera = false;
		  private static bool blackOverlayCamera = false;
		  private static bool halfTone = false;
		  private static bool onOf = false;

		  private static bool[] options;
		  private static Spirals spiralOption;
		  private static Triangles triangleOption;

		  private static Color mainColor;
		  private static Color subColor;

		  private static Camera[] cameras = new Camera[4];

		  private static void CreateLineMaterial() {
				if (!lineMaterial) {
					 Shader shader = Shader.Find("Hidden/Internal-Colored");
					 lineMaterial = new Material(shader);
					 lineMaterial.hideFlags = HideFlags.HideAndDontSave;
					 lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
					 lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
					 lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
					 lineMaterial.SetInt("_ZWrite", 0);
				}
		  }

		  private static Camera CreateCamera(int targetDisplay) {
				var go = new GameObject();
				var camera = go.AddComponent<Camera>();
				camera.name = "pp cam";
				camera.targetDisplay = targetDisplay;
				camera.cullingMask = 0;
				camera.depth = 100;
				camera.clearFlags = CameraClearFlags.Depth;
				go.hideFlags = HideFlags.HideAndDontSave;
				return camera;
		  }

		  /// <summary>
		  /// Quite fast, about 10-20 ticks
		  /// </summary>
		  private static void CheckCameras() {
				for (int i = 0; i < 4; ++i) {
					 if (cameras[i] == null) {
						  bool found = false;
						  for(int j = 0; j < Camera.allCamerasCount; ++j) {
								var cam = Camera.allCameras[j];
								if(cam.targetDisplay == i && cam.depth == 100) {
									 cameras[i] = cam;
									 found = true;
									 break;
								}
						  }
						  if (!found) {
								cameras[i] = CreateCamera(i);
						  }
					 }
				}
		  }

		  private static void OnPostRender(Camera camera) {
				if (!onOf) {
					 return;
				}

				if (camera.name == "SceneCamera") {
					 if (overlayOnlyOnMainCamera) {
						  return;
					 }
				} else {
					 if (camera.name != "pp cam") {
						  return;
					 }
				}

				CreateLineMaterial();
				GL.PushMatrix();
				lineMaterial.SetPass(0);
				GL.LoadOrtho();
				GL.Begin(GL.LINES);

				if (blackOverlayCamera) {
					 mainColor = new Color(0, 0, 0, 0.85f * (halfTone ? 0.33333f : 1));
				} else {
					 mainColor = new Color(1, 1, 1, 0.66666f * (halfTone ? 0.15f : 1));
				}
				subColor = mainColor;
				subColor.a = subColor.a / 2;

				GL.Color(mainColor);

				// golden ratio
				if (options[(int)Options.GoldenRatio]) {
					 GLVerticalLine(phi - 1);
					 GLVerticalLine(2 - phi);
					 GLHorizontalLine(phi - 1);
					 GLHorizontalLine(2 - phi);
				}
				// diagonal
				if (options[(int)Options.Diagonal]) {
					 GLLine(new Vector3(0, 0), new Vector3(1, 1));
					 GLLine(new Vector3(1, 0), new Vector3(0, 1));
				}
				// thirds
				if (options[(int)Options.Thirds]) {
					 GLVerticalLine(1 / 3.0f);
					 GLVerticalLine(2 / 3.0f);
					 GLHorizontalLine(1 / 3.0f);
					 GLHorizontalLine(2 / 3.0f);
				}
				// ps4 GUI
				if (options[(int)Options.Ps4GUI]) {
					 GLLine(new Vector3(0.05f, 0.05f), new Vector3(0.95f, 0.05f));
					 GLLine(new Vector3(0.95f, 0.05f), new Vector3(0.95f, 0.95f));
					 GLLine(new Vector3(0.95f, 0.95f), new Vector3(0.05f, 0.95f));
					 GLLine(new Vector3(0.05f, 0.95f), new Vector3(0.05f, 0.05f));
				}
				// spiral
				if (options[(int)Options.Spiral]) {
					 switch (spiralOption) {
						  case Spirals.LeftTop:
								DrawFibRect(new Rect(0, 0, 1, 1), 2);
								break;
						  case Spirals.RightDown:
								DrawFibRect(new Rect(0, 0, 1, 1), 0);
								break;
						  case Spirals.TopRight:
								DrawFibRect(new Rect(0, 0, 1, 1), 3);
								break;
						  case Spirals.DownLeft:
								DrawFibRect(new Rect(0, 0, 1, 1), 1);
								break;
					 }
				}
				// dragon
				if (options[(int)Options.Dragon]) {
					 dragonIT = (int)Mathf.Clamp(dragonIT, 1, 14);
					 DrawDragonLine(dragonIT, true, 0.33f, 0.33f, 0.25f, 0.25f);
				}
				// Triangle
				if (options[(int)Options.Triangle]) {
					 switch (triangleOption) {
						  case Triangles.Golden:
								GoldenTriangle(camera);
								break;
						  case Triangles.Harmonious:
								HarmoniousTriangle(camera);
								break;
					 }
				}

				GL.End();
				GL.PopMatrix();
		  }

		  private static void HarmoniousTriangle(Camera camera) {
				GLLine(new Vector2(0, 1), new Vector2(1, 0));

				float height = camera.pixelHeight;
				float width = camera.pixelWidth;
				float tanAlfa = height / width;
				float radians = Mathf.Atan(tanAlfa);
				float alfaAngle = radians * (180.0f / Mathf.PI);
				float gammaAngle = 90 - alfaAngle;
				float betaAngle = 180 - 90 - gammaAngle;
				float length = Mathf.Cos(betaAngle * (Mathf.PI / 180.0f)) * height;

				Vector3 startPointLeftBottom = new Vector2(0, 0);
				Vector3 endPointLeftBottom = new Vector2(
					 startPointLeftBottom.x + Mathf.Cos((90 - betaAngle) * (Mathf.PI / 180.0f)) * length / width,
					 startPointLeftBottom.y + Mathf.Sin((90 - betaAngle) * (Mathf.PI / 180.0f)) * length / height);

				GLLine(startPointLeftBottom, endPointLeftBottom);

				Vector3 startPointRightTop = new Vector2(1, 1);
				Vector3 endPointRightTop = new Vector2(
					 startPointRightTop.x + Mathf.Cos((270 - betaAngle) * (Mathf.PI / 180.0f)) * length / width,
					 startPointRightTop.y + Mathf.Sin((270 - betaAngle) * (Mathf.PI / 180.0f)) * length / height);

				GLLine(startPointRightTop, endPointRightTop);
		  }

		  private static void GoldenTriangle(Camera camera) {
				GLLine(new Vector2(0, 0), new Vector2(1, 1));

				float height = camera.pixelHeight;
				float width = camera.pixelWidth;
				float tanAlfa = height / width;
				float radians = Mathf.Atan(tanAlfa);
				float alfaAngle = radians * (180.0f / Mathf.PI);
				float gammaAngle = 90 - alfaAngle;
				float betaAngle = 180 - 90 - gammaAngle;
				float length = Mathf.Cos(betaAngle * (Mathf.PI / 180.0f)) * height;

				Vector3 startPointLeftTop = new Vector2(0, 1);
				Vector3 endPointLeftTop = new Vector2(
					 startPointLeftTop.x + Mathf.Cos((270 + betaAngle) * (Mathf.PI / 180.0f)) * length / width,
					 startPointLeftTop.y + Mathf.Sin((270 + betaAngle) * (Mathf.PI / 180.0f)) * length / height);

				GLLine(startPointLeftTop, endPointLeftTop);

				Vector3 startPointRightBottom = new Vector2(1, 0);
				Vector3 endPointRightBottom = new Vector2(
					 startPointRightBottom.x + Mathf.Cos((90 + betaAngle) * (Mathf.PI / 180.0f)) * length / width,
					 startPointRightBottom.y + Mathf.Sin((90 + betaAngle) * (Mathf.PI / 180.0f)) * length / height);

				GLLine(startPointRightBottom, endPointRightBottom);
		  }

		  static void DrawDragonLine(int level, bool left, float x1, float y1, float dx, float dy) {
				if (level <= 0) {
					 GLLine(new Vector3(x1, y1), new Vector3(x1 + dx, y1 + dy));
				} else {
					 float nx = (float)(dx / 2);
					 float ny = (float)(dy / 2);
					 float dx2 = -ny + nx;
					 float dy2 = nx + ny;
					 if (!left) {
						  DrawDragonLine(level - 1, false,
								  x1, y1, dx2, dy2);
						  float x2 = x1 + dx2;
						  float y2 = y1 + dy2;
						  DrawDragonLine(level - 1, true,
								  x2, y2, dy2, -dx2);
					 } else {
						  DrawDragonLine(level - 1, false,
								  x1, y1, dy2, -dx2);
						  float x2 = x1 + dy2;
						  float y2 = y1 - dx2;
						  DrawDragonLine(level - 1, true,
								  x2, y2, dx2, dy2);
					 }
				}
		  }

		  static void DrawFibRect(Rect rect, int rotation) {
				var mainRect = new Rect(rect);
				var reminderRect = new Rect(rect);
				if (rotation % 4 == 0) {
					 mainRect.width = mainRect.width * (phi - 1);
					 reminderRect.width = rect.width - mainRect.width;
					 reminderRect.x = mainRect.xMax;
				}
				if (rotation % 4 == 1) {
					 mainRect.height = mainRect.height * (phi - 1);
					 mainRect.y = rect.yMax - mainRect.height;
					 reminderRect.height = rect.height - mainRect.height;
				}
				if (rotation % 4 == 2) {
					 mainRect.width = mainRect.width * (phi - 1);
					 mainRect.x = rect.xMax - mainRect.width;
					 reminderRect.width = rect.width - mainRect.width;
				}
				if (rotation % 4 == 3) {
					 mainRect.height = mainRect.height * (phi - 1);
					 reminderRect.height = rect.height - mainRect.height;
					 reminderRect.y = mainRect.yMax;
				}
				DrawSquereArc(mainRect, rotation);
				GL.Color(subColor);
				GLDrawRect(mainRect);
				GL.Color(mainColor);
				if (rotation > 12) {
					 return;
				}
				DrawFibRect(reminderRect, rotation + 1);
		  }

		  static void DrawSquereArc(Rect rect, int rotation) {
				Vector3 a = rect.min;
				Vector3 b = rect.max;
				float w = Mathf.Abs(b.x - a.x);
				float h = Mathf.Abs(b.y - a.y);
				rotation = rotation % 4;
				switch (rotation) {
					 case 0:
						  DrawArc(180, 90, w, h, new Vector3(b.x, a.y));
						  break;
					 case 1:
						  DrawArc(90, 0, w, h, a);
						  break;
					 case 2:
						  DrawArc(0, -90, w, h, new Vector3(a.x, b.y));
						  break;
					 case 3:
						  DrawArc(-90, -180, w, h, b);
						  break;
					 default:
						  break;
				}
		  }

		  // a & b in degrees
		  static void DrawArc(float a, float b, float width, float height, Vector3 center) {
				a = Mathf.PI * a / 180;
				b = Mathf.PI * b / 180;
				int steps = 24;
				var previousVec = new Vector3(width * Mathf.Cos(a) + center.x, height * Mathf.Sin(a) + center.y);
				for (int i = 1; i < steps; ++i) {
					 float t = Mathf.Lerp(a, b, (float)i / (steps - 1));
					 GL.Vertex(previousVec);
					 var vec = new Vector3(width * Mathf.Cos(t) + center.x, height * Mathf.Sin(t) + center.y);
					 GL.Vertex(vec);
					 previousVec = vec;
				}
		  }

		  static void GLLine(Vector3 start, Vector3 end) {
				GL.Vertex(start);
				GL.Vertex(end);
		  }

		  static void GLVerticalLine(float pos) {
				var vertex = new Vector3(pos, 0, 0);
				GL.Vertex(vertex);
				vertex.y = 1;
				GL.Vertex(vertex);
		  }

		  static void GLHorizontalLine(float pos) {
				var vertex = new Vector3(0, pos, 0);
				GL.Vertex(vertex);
				vertex.x = 1;
				GL.Vertex(vertex);
		  }

		  static void GLDrawRect(Rect rect) {
				var v0 = rect.min;
				var v1 = rect.min;
				v1.x = rect.max.x;
				GLLine(v0, v1);
				v0 = v1;
				v1.y = rect.max.y;
				GLLine(v0, v1);
				v0 = v1;
				v1.x = rect.min.x;
				GLLine(v0, v1);
				v0 = v1;
				v1 = rect.min;
				GLLine(v0, v1);
		  }

		  static void DoOverlayMenu() {
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("on off"), onOf, () => { onOf = !onOf; Save(); });
				menu.AddItem(new GUIContent("only for main Camera"), overlayOnlyOnMainCamera, () => { overlayOnlyOnMainCamera = !overlayOnlyOnMainCamera; Save(); });
				menu.AddSeparator("");
				menu.AddItem(new GUIContent("golden ratio"), options[(int)Options.GoldenRatio],
					  () => { SwapOverlayCameraOption(Options.GoldenRatio); Save(); });
				menu.AddItem(new GUIContent("diagonal"), options[(int)Options.Diagonal],
					  () => { SwapOverlayCameraOption(Options.Diagonal); Save(); });
				menu.AddItem(new GUIContent("thirds"), options[(int)Options.Thirds],
					  () => { SwapOverlayCameraOption(Options.Thirds); Save(); });
				menu.AddItem(new GUIContent("ps4 GUI"), options[(int)Options.Ps4GUI],
					  () => { SwapOverlayCameraOption(Options.Ps4GUI); Save(); });
				menu.AddItem(new GUIContent("spiral"), options[(int)Options.Spiral],
					  () => { SwapOverlayCameraOption(Options.Spiral); Save(); });

				menu.AddItem(new GUIContent("spiral rotation/RightDown"), spiralOption == Spirals.RightDown,
					  () => { spiralOption = Spirals.RightDown; Save(); });
				menu.AddItem(new GUIContent("spiral rotation/LeftTop"), spiralOption == Spirals.LeftTop,
					 () => { spiralOption = Spirals.LeftTop; Save(); });
				menu.AddItem(new GUIContent("spiral rotation/TopRight"), spiralOption == Spirals.TopRight,
					 () => { spiralOption = Spirals.TopRight; Save(); });
				menu.AddItem(new GUIContent("spiral rotation/DownLeft"), spiralOption == Spirals.DownLeft,
					 () => { spiralOption = Spirals.DownLeft; Save(); });

				menu.AddItem(new GUIContent("dragon"), options[(int)Options.Dragon],
					  () => { SwapOverlayCameraOption(Options.Dragon); Save(); });

				menu.AddItem(new GUIContent("dragon +"), false,
					  () => { dragonIT++; });
				menu.AddItem(new GUIContent("dragon -"), false,
					  () => { dragonIT--; });
				menu.AddItem(new GUIContent("triangle"), options[(int)Options.Triangle],
					 () => { SwapOverlayCameraOption(Options.Triangle); Save(); });
				menu.AddItem(new GUIContent("triangle variant/Golden"), triangleOption == Triangles.Golden,
					 () => { triangleOption = Triangles.Golden; Save(); });
				menu.AddItem(new GUIContent("triangle variant/Harmonious"), triangleOption == Triangles.Harmonious,
					 () => { triangleOption = Triangles.Harmonious; Save(); });


				menu.AddSeparator("");
				menu.AddItem(new GUIContent("black"), blackOverlayCamera, () => { blackOverlayCamera = !blackOverlayCamera; Save(); });
				menu.AddItem(new GUIContent("half tone"), halfTone, () => { halfTone = !halfTone; Save(); });
				menu.AddItem(new GUIContent("#cameras in scene: " + Camera.allCamerasCount.ToString()), halfTone, () => { });
				menu.ShowAsContext();
		  }

		  static void SwapOverlayCameraOption(Options option) {
				var result = !options[(int)option];
				options[(int)option] = result;
				onOf = onOf | result;
		  }

		  static public void Init() {
				Camera.onPostRender -= OnPostRender;
				Camera.onPostRender += OnPostRender;
				options = new bool[System.Enum.GetValues(typeof(Options)).Length];
				DefaultOverlayCameraTools.overlayCameraContent = new GUIContent(IconTextureCache.Get("OverlayCamera"), "Overlay (grids etc)");
				Load();
		  }

		  static public void Draw() {
				var overlayCamera = Layout.Button(overlayCameraContent);
				if (overlayCamera) {
					 DoOverlayMenu();
				}
				CheckCameras();
		  }

		  static private void Load() {
				DefaultOverlayCameraTools.onOf = UnityEditor.EditorPrefs.GetBool("doct/onOf", false);
				DefaultOverlayCameraTools.blackOverlayCamera = UnityEditor.EditorPrefs.GetBool("doct/black overlay camera", false);
				DefaultOverlayCameraTools.halfTone = UnityEditor.EditorPrefs.GetBool("doct/half tone", false);
				DefaultOverlayCameraTools.overlayOnlyOnMainCamera = UnityEditor.EditorPrefs.GetBool("doct/only main camera", false);
				DefaultOverlayCameraTools.spiralOption = (Spirals)UnityEditor.EditorPrefs.GetInt("doct/spiral option", 0);
				DefaultOverlayCameraTools.triangleOption = (Triangles)UnityEditor.EditorPrefs.GetInt("doct/triangle option", 0);
				for (int i = 0; i < options.Length; ++i) {
					 options[i] = UnityEditor.EditorPrefs.GetBool("doct/options" + i.ToString(), i == 0);
				}
		  }

		  static private void Save() {
				UnityEditor.EditorPrefs.SetBool("doct/onOf", onOf);
				UnityEditor.EditorPrefs.SetBool("doct/black overlay camera", blackOverlayCamera);
				UnityEditor.EditorPrefs.SetBool("doct/half tone", halfTone);
				UnityEditor.EditorPrefs.SetBool("doct/only main camera", overlayOnlyOnMainCamera);
				UnityEditor.EditorPrefs.SetInt("doct/spiral option", (int)spiralOption);
				UnityEditor.EditorPrefs.SetInt("doct/triangle option", (int)triangleOption);
				for (int i = 0; i < options.Length; ++i) {
					 UnityEditor.EditorPrefs.SetBool("doct/options" + i.ToString(), options[i]);
				}
		  }
	 };

	 public static class DefaultLayerTools {
		  static private int LayerNameToInt(string name) {
				return 1 << LayerMask.NameToLayer(name);
		  }

		  static private int UI_LAYER = LayerNameToInt("UI");
		  static private GUIContent[] contents = new GUIContent[] { new GUIContent("UI", "Show only UI"), new GUIContent("~UI", "Exclude UI"), new GUIContent("All", "Show all") };

		  static public void Draw() {
				var tool = Layout.Toolbar(contents, -1);
				if (tool == 0) {
					 Tools.visibleLayers = UI_LAYER;
				}
				if (tool == 1) {
					 Tools.visibleLayers = int.MaxValue & (int.MaxValue - UI_LAYER);
				}
				if (tool == 2) {
					 Tools.visibleLayers = int.MaxValue;
				}
		  }
	 };

	 public static class DefaultTimeScaleTool {
		  static public void Draw() {
				var timeScale = GUILayout.HorizontalSlider(Time.timeScale, 0, 2, GUILayout.Width(100));
				Time.timeScale = (Mathf.RoundToInt(timeScale * 10)) / 10f;
				var resetTime = Layout.Button("Scale: " + Time.timeScale, 60);
				if (resetTime) {
					 Time.timeScale = 1;
				}
		  }
	 }

	 public static class DefaultIsolateTool {
		  static private GUIContent onIsolateContent;
		  static private GUIContent offIsolateContent;

		  private static List<Transform> s_HideContent = null;

		  /// <summary>
		  /// Turn on transforms with checking if were destroyed
		  /// </summary>
		  /// <param name="transforms"></param>
		  /// <param name="value"></param>
		  static public void SetActive(List<Transform> transforms, bool value) {
				if (transforms != null) {
					 foreach (var node in transforms) {
						  if (node != null) {
								node.gameObject.SetActive(value);
						  }
					 }
				}
		  }

		  static public void Init() {
				onIsolateContent = new GUIContent(IconTextureCache.Get("Isolate"), "turn on isolate based on current selection root");
				offIsolateContent = new GUIContent(IconTextureCache.Get("Isolate_On"), "turn off isolate");
		  }

		  static public void Isolate(GameObject[] selection, ref List<Transform> hideCache, List<Transform> allRoots = null) {
				List<Transform> selectedTransforms = new List<Transform>();
				List<Transform> neededTransforms = new List<Transform>();
				List<Transform> toHide = new List<Transform>();

				foreach (var go in selection) {
					 if (!string.IsNullOrEmpty(go.scene.name)) {
						  selectedTransforms.Add(go.transform);
					 }
				}

				foreach (var transform in selectedTransforms) {
					 neededTransforms.AddRange(transform.GetComponentsInChildren<Transform>(false));
					 neededTransforms.AddRange(transform.GetComponentsInParent<Transform>(false));
					 var rootChildren = transform.root.GetComponentsInChildren<Transform>(false);
					 if (allRoots != null) {
						  allRoots.AddRange(rootChildren);
						  rootChildren = allRoots.ToArray();
					 }
					 for (int i = 0; i < rootChildren.Length; ++i) {
						  if (!toHide.Contains(rootChildren[i])) {
								toHide.Add(rootChildren[i]);
						  }
					 }
				}

				foreach (var neededTransform in neededTransforms) {
					 toHide.Remove(neededTransform);
				}

				hideCache = toHide;
				var allreadyHided = new List<Transform>();
				foreach(var transform in hideCache) {
					 if (!transform.gameObject.activeInHierarchy) {
						  allreadyHided.Add(transform);
					 }
				}
				foreach(var transform in allreadyHided) {
					 hideCache.Remove(transform);
				}
				SetActive(hideCache, false);
		  }

		  static private void Hide() {
				var selection = new List<GameObject>();
				selection.AddRange(Selection.gameObjects);
				selection.RemoveAll((x) => string.IsNullOrEmpty(x.scene.name));
				var roots = new List<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
				var allRoots = new List<Transform>();
				foreach (var root in roots) {
					 allRoots.Add(root.transform);
				}
				foreach (var root in roots) {
					 var components = new List<Component>();
					 components.AddRange(root.GetComponentsInChildren<Camera>(false));
					 components.AddRange(root.GetComponentsInChildren<ReflectionProbe>(false));
					 components.AddRange(root.GetComponentsInChildren<Light>(false));
					 foreach (var component in components) {
						  selection.Add(component.gameObject);
					 }
				}
				Isolate(selection.ToArray(), ref s_HideContent, allRoots);
		  }

		  static private void Unhide() {
				SetActive(s_HideContent, true);
				s_HideContent = null;
		  }

		  static public void Draw() {
				var isHiden = s_HideContent != null;
				var hide = Layout.Toggle(isHiden ? offIsolateContent : onIsolateContent, isHiden);
				if (hide != isHiden) {
					 if (hide) {
						  Hide();
					 } else {
						  Unhide();
					 }
				}
		  }
	 };

	 public static class SmartTools {
		  static public void Draw() {
				var popup = Layout.Button("T", 15);
				if (popup) {
					 GenericMenu gm = new GenericMenu();
					 gm.AddItem(new GUIContent("Re Root"), false, ReRoot);
					 gm.AddItem(new GUIContent("Smart Rename"), false, SmartRename);
					 gm.ShowAsContext();
				}
		  }

		  static public void SmartRename() {
				var gameObjects = Selection.gameObjects;
				foreach(var go in gameObjects) {
					 var textComponent = go.GetComponentInChildren<UnityEngine.UI.Text>();
					 if(textComponent != null) {
						  var name = textComponent.text.ToLower();
						  var charArray = name.ToCharArray();
						  if (charArray.Length > 0) {
								charArray[0] = name.ToUpper()[0];
								go.name = new string(charArray);
						  }
					 }
				}
		  }

		  static public void ReRoot() {
				var gameObjects = Selection.gameObjects;
				foreach(var go in gameObjects) {
					 var newGo = new GameObject(go.name);
					 newGo.layer = go.layer;
					 if(go.transform is RectTransform) {
						  newGo.AddComponent<RectTransform>();
					 }
					 newGo.transform.SetParent(go.transform.parent, false);
					 var rectTransform = newGo.transform as RectTransform;
					 if(rectTransform != null) {
						  UnityEditorInternal.ComponentUtility.CopyComponent(go.transform as RectTransform);
						  UnityEditorInternal.ComponentUtility.PasteComponentValues(rectTransform);
					 }
					 go.transform.SetParent(newGo.transform, false);
				}
		  }
	 }

	 [InitializeOnLoad]
	 public static class DefaultTools {
		  static DefaultTools() {
				DefaultIsolateTool.Init();
				//DefaultOverlayCameraTools.Init();
				DefaultCemeraTools.Init();
				System.Action defaultTools = () => {
					 GUILayout.Space(4);
					 SmartTools.Draw();
					 GUILayout.Space(4);
					 DefaultLayerTools.Draw();
					 GUILayout.Space(4);
					 DefaultIsolateTool.Draw();
					 GUILayout.Space(4);
					 DefaultCemeraTools.Draw();
					 //DefaultOverlayCameraTools.Draw();
					 GUILayout.Space(4);
					 DefaultTimeScaleTool.Draw();
					 GUILayout.Space(4);
				};
				Controller.topBarDrawers.AddListener(defaultTools, 0);
		  }

	 };

}// end of namespace