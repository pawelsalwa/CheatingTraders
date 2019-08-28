using UnityEngine;
using UnityEditor;

namespace AWI.SceneViewTools {
	 public static class Layout {
		  private static GUIStyle m_button;

		  public static GUIStyle button {
				get {
					 if (m_button == null) {
						  m_button = new GUIStyle(EditorStyles.toolbarButton);
						  m_button.fontSize = 8;
						  m_button.fontStyle = FontStyle.Bold;
					 }
					 return m_button;
				}
		  }

		  private static GUIStyle m_popup;

		  public static GUIStyle popup {
				get {
					 if (m_popup == null) {
						  m_popup = new GUIStyle(EditorStyles.toolbarPopup);
						  m_popup.fontSize = 8;
						  m_popup.fontStyle = FontStyle.Bold;
					 }
					 return m_popup;
				}
		  }

		  private static GUIStyle m_miniLabel;

		  public static GUIStyle label {
				get {
					 if (m_miniLabel == null) {
						  m_miniLabel = new GUIStyle(EditorStyles.whiteMiniLabel);
						  m_miniLabel.fontSize = 8;
						  m_miniLabel.fontStyle = FontStyle.Bold;
						  m_miniLabel.normal.textColor = popup.normal.textColor;
						  m_miniLabel.alignment = popup.alignment;
						  m_miniLabel.padding = popup.padding;
					 }
					 return m_miniLabel;
				}
		  }

		  public static int Popup(string label, string[] displayedOptions, int selected, int width = -1) {
				int result = -1;
				if (width == -1) {
					 result = EditorGUILayout.Popup(selected, displayedOptions, popup, GUILayout.ExpandWidth(false));
				} else {
					 result = EditorGUILayout.Popup(selected, displayedOptions, popup, GUILayout.Width(width));
				}
				var rect = GUILayoutUtility.GetLastRect();
				GUI.Label(rect, label, Layout.label);
				return result;
		  }

		  public static bool Button(GUIContent content, int width = -1) {
				if (width == -1) {
					 return GUILayout.Button(content, button, GUILayout.ExpandWidth(false));
				} else {
					 return GUILayout.Button(content, button, GUILayout.Width(width));
				}
		  }

		  public static bool Button(string name, int width = -1) {
				if (width == -1) {
					 return GUILayout.Button(name, button, GUILayout.ExpandWidth(false));
				} else {
					 return GUILayout.Button(name, button, GUILayout.Width(width));
				}
		  }


		  public static bool Toggle(GUIContent content, bool value, int width = -1) {
				if (width == -1) {
					 return GUILayout.Toggle(value, content, button, GUILayout.ExpandWidth(false));
				} else {
					 return GUILayout.Toggle(value, content, button, GUILayout.Width(width));
				}
		  }

		  public static bool Toggle(string name, bool value, int width = -1) {
				if (width == -1) {
					 return GUILayout.Toggle(value, name, button, GUILayout.ExpandWidth(false));
				} else {
					 return GUILayout.Toggle(value, name, button, GUILayout.Width(width));
				}
		  }

		  public static void Label(string label, int width = -1) {
				if (width == -1) {
					 GUILayout.Label(label, button, GUILayout.ExpandWidth(false));
				} else {
					 GUILayout.Label(label, button, GUILayout.Width(width));
				}
		  }

		  public static int Toolbar(GUIContent[] contents, int selected, int width = -1) {
				if (width == -1) {
					 return GUILayout.Toolbar(selected, contents, button, GUILayout.ExpandWidth(false));
				} else {
					 return GUILayout.Toolbar(selected, contents, button);
				}
		  }

	 }

	 [InitializeOnLoad]
	 public class Controller {
		  static bool init = false;
		  static bool lazy_showOtherTools;
		  static bool lazy_collapse;
		  static bool lazy_showOnTop;

		  public static SceneView.OnSceneFunc nonTulbarDelegate = null;
		  public static EventPriority topBarDrawers = new EventPriority();

		  static private Rect topBarRect {
				get {
					 return new Rect(0, 0, Screen.width, 25);
				}
		  }

		  static private Rect bottomBarRect {
				get {
					 return new Rect(0, Screen.height - 56, Screen.width, 25);
				}
		  }

		  static bool collapse {
				get {
					 Init();
					 return lazy_collapse;
				}
				set {
					 if (value != lazy_collapse) {
						  lazy_collapse = value;
						  EditorPrefs.SetBool("collapse scene view tools", lazy_showOtherTools);
					 }
				}
		  }

		  static bool showOnTop {
				get {
					 Init();
					 return lazy_showOnTop;
				}
				set {
					 if (value != lazy_showOnTop) {
						  lazy_showOnTop = value;
						  EditorPrefs.SetBool("show on top scene view tools", lazy_showOnTop);
					 }
				}
		  }
		  
		  static Controller() {
				SceneView.onSceneGUIDelegate -= DoOnGUI;
				SceneView.onSceneGUIDelegate += DoOnGUI;
		  }

		  static void Init() {
				if (!init) {
					 lazy_showOtherTools = EditorPrefs.GetBool("show other tools", false);
					 lazy_collapse = EditorPrefs.GetBool("collapse scene view tools", false);
					 lazy_showOnTop = EditorPrefs.GetBool("show on top scene view tools", true);
					 init = true;
				}
		  }
		  
		  static private void DrawMainBar(SceneView sceneview) {
				var rect = showOnTop ? topBarRect : bottomBarRect;
				GUILayout.BeginArea(rect);
				GUILayout.BeginHorizontal();
				if (Layout.Button(collapse ? SpecChars.next : SpecChars.previous)){
					 collapse = !collapse;
				}
				if (!collapse) {
					 if (Layout.Button(showOnTop ? SpecChars.down : SpecChars.up)) {
						  showOnTop = !showOnTop;
					 }
					 topBarDrawers.Invoke();
				}
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
		  }

		  static private void DoOnGUI(SceneView sceneview) {
				if (nonTulbarDelegate != null) {
					 nonTulbarDelegate(sceneview);
				}
				Handles.BeginGUI();
				DrawMainBar(sceneview);
				Handles.EndGUI();
		  }
	 }
}
