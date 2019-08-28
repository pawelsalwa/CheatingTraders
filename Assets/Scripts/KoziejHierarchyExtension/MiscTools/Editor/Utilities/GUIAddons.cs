using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace AWI {

	 public static class Labels {
		  private static Dictionary<string, GUIStyle> cachedLabels = new Dictionary<string, GUIStyle>();

		  private static string LabelHash(int size, Color color, TextAnchor aligment) {
				return size.ToString() + " " + ((int)(255 * color.r + 255 * 10 * color.g + 255 * 100 * color.b + 255 * 1000 * color.a)).ToString() + " " + ((int)aligment).ToString();
		  }

		  public static GUIStyle leftGreyMiniLabel {
				get {
					 return Label(10, EditorStyles.centeredGreyMiniLabel.normal.textColor, TextAnchor.MiddleLeft);
				}
		  }

		  public static GUIStyle Label(int size, Color color, TextAnchor aligment) {
				var hash = LabelHash(size, color, aligment);
				var result = null as GUIStyle;
				if (!cachedLabels.TryGetValue(hash, out result)) {
					 result = new GUIStyle(EditorStyles.miniLabel);
					 result.fontSize = size;
					 result.normal.textColor = color;
					 result.alignment = aligment;
					 cachedLabels.Add(hash, result);
				}
				return result;
		  }

		  public static GUIStyle WhiteLabel(int size, TextAnchor aligment) {
				return Label(size, Color.white, aligment);
		  }

		  public static GUIStyle BlackLabel(int size, TextAnchor aligment) {
				return Label(size, Color.black, aligment);
		  }

		  public static GUIStyle Label(int size, TextAnchor aligment) {
				var color = EditorStyles.label.normal.textColor;
				return Label(size, color, aligment);
		  }
	 }

	 public static class MiscStyles {
		  static GUIStyle m_personal_Box;
		  static GUIStyle m_pro_Box;

		  public static GUIStyle box {
				get {
					 if (EditorGUIUtility.isProSkin) {
						  if (m_personal_Box == null) {
								m_personal_Box = new GUIStyle(EditorStyles.helpBox);
								m_personal_Box.fontSize = 8;
						  }
						  return m_personal_Box;
					 } else {
						  if (m_pro_Box == null) {
								m_pro_Box = new GUIStyle(EditorStyles.helpBox);
								m_pro_Box.fontSize = 8;
						  }
						  return m_pro_Box;
					 }
				}
		  }

		  public static GUIStyle presetChooserStyle = "IN ObjectField";
		  public static GUIStyle shurikenToggle = "ShurikenToggle";
		  public static GUIStyle shurikenLabel = "ShurikenLabel";

		  public static GUIStyle lockButton = DoLockButton();

		  public static GUIStyle DoLockButton() {
				var style = new GUIStyle("IN LockButton");
				style.overflow = new RectOffset(0, 0, -2, 2);
				return style;
		  }

		  public static GUIStyle leftButton {
				get {
					 return GUI.skin.button.name + "Left";
				}
		  }
		  public static GUIStyle midButton {
				get {
					 return GUI.skin.button.name + "Mid";
				}
		  }
		  public static GUIStyle rightButton {
				get {
					 return GUI.skin.button.name + "Right";
				}
		  }

		  public static GUIStyle ToolbarButton(int pos, int count) {
				if (count == 1) {
					 return GUI.skin.button;
				} else {
					 if (pos == 0) {
						  return leftButton;
					 } else if (pos == count - 1) {
						  return rightButton;
					 } else {
						  return midButton;
					 }
				}
		  }

		  public static GUIStyle MiniToolbarButton(int pos, int count) {
				if (count == 1) {
					 return EditorStyles.miniButton;
				} else {
					 if (pos == 0) {
						  return EditorStyles.miniButtonLeft;
					 } else if (pos == count - 1) {
						  return EditorStyles.miniButtonRight;
					 } else {
						  return EditorStyles.miniButtonMid;
					 }
				}
		  }

		  private static GUIStyle lazy_EasyHilight;

		  public static GUIStyle easyHilight {
				get {
					 if (lazy_EasyHilight == null) {
						  lazy_EasyHilight = new GUIStyle();
						  lazy_EasyHilight.hover = new GUIStyleState();
						  lazy_EasyHilight.hover.background = Texture2D.whiteTexture;
						  lazy_EasyHilight.hover.textColor = Color.white;
						  lazy_EasyHilight.normal.textColor = Color.clear;
						  lazy_EasyHilight.alignment = TextAnchor.MiddleCenter;
					 }
					 return lazy_EasyHilight;
				}
		  }
	 };

	 public static class GUIAddons {

		  private static Color hilightColor = new Color(0, 0, 1, 0.5f);

		  private static GUILayoutOption[] drawOpenCloseButtonGUILayoutOption = new GUILayoutOption[] {
				GUILayout.Width(110f)
		  };

		  public class ToolbarContent {
				private GUIContent[] lazy_toolbarContentsPro = null;
				private GUIContent[] lazy_toolbarContentsPersonal = null;
				private GUIContent[] lazy_toolbarContentsPressed = null;

				static private string suffix_pro = "_Normal_Pro";
				static private string suffix_personal = "_Normal_Personal";
				static private string suffix_pressed = "_Pressed";

				[System.NonSerialized]
				private bool inicialized = false;

				private string[] names;
				private string[] tooltips;
				private string path;

				static private GUIContent[] doToolbarContents(string path, string[] names, string[] tooltips, string suffix) {
					 var result = new GUIContent[names.Length];
					 for (int i = 0; i < result.Length; ++i) {
						  result[i] = new GUIContent(IconTextureCache.Get(names[i] + suffix), tooltips[i]);
					 }
					 return result;
				}

				/// <summary>
				/// Texture finder use "ET_" preffix
				/// </summary>
				/// <param name="path"></param>
				/// <param name="names"></param>
				/// <param name="tooltips"></param>
				public ToolbarContent(string path, string[] names, string[] tooltips) {
					 this.names = names;
					 this.path = path;
					 this.tooltips = tooltips;
				}

				public GUIContent[] GetContents(bool pressed) {
					 if (!inicialized) {
						  lazy_toolbarContentsPro = doToolbarContents(path, names, tooltips, suffix_pro);
						  lazy_toolbarContentsPersonal = doToolbarContents(path, names, tooltips, suffix_personal);
						  lazy_toolbarContentsPressed = doToolbarContents(path, names, tooltips, suffix_pressed);
						  inicialized = true;
					 }
					 if (pressed) {
						  return lazy_toolbarContentsPressed;
					 } else {
						  if (EditorGUIUtility.isProSkin) {
								return lazy_toolbarContentsPro;
						  } else {
								return lazy_toolbarContentsPersonal;
						  }
					 }
				}
		  }

		  public class LabelWdithScope : IDisposable {
				private float cache;

				public LabelWdithScope(float width) {
					 cache = EditorGUIUtility.labelWidth;
					 EditorGUIUtility.labelWidth = width < 0 ? EditorGUIUtility.labelWidth + width : width;
				}

				public void Dispose() {
					 EditorGUIUtility.labelWidth = cache;
				}
		  }

		  public class FieldWdithScope : IDisposable {
				private float cache;

				public FieldWdithScope(float width) {
					 cache = EditorGUIUtility.fieldWidth;
					 EditorGUIUtility.fieldWidth = width < 0 ? EditorGUIUtility.fieldWidth + width : width;
				}

				public void Dispose() {
					 EditorGUIUtility.fieldWidth = cache;
				}
		  }

		  public class LabelFieldWdithScope : IDisposable {
				private float fieldCache;
				private float labelCache;

				public LabelFieldWdithScope(float labelWidth, float fieldWidth) {
					 fieldCache = EditorGUIUtility.fieldWidth;
					 labelCache = EditorGUIUtility.labelWidth;
					 EditorGUIUtility.labelWidth = labelWidth < 0 ? EditorGUIUtility.labelWidth + labelWidth : labelWidth;
					 EditorGUIUtility.fieldWidth = fieldWidth < 0 ? EditorGUIUtility.fieldWidth + fieldWidth : fieldWidth;
				}

				/// <summary>
				/// Usage for cache width and field values
				/// </summary>
				public LabelFieldWdithScope() {
					 fieldCache = EditorGUIUtility.fieldWidth;
					 labelCache = EditorGUIUtility.labelWidth;
				}

				public void Dispose() {
					 EditorGUIUtility.fieldWidth = fieldCache;
					 EditorGUIUtility.labelWidth = labelCache;
				}
		  }

		  public class ColorBackgroundScope : IDisposable {
				private Color cache;

				public ColorBackgroundScope(Color color) {
					 cache = GUI.backgroundColor;
					 GUI.backgroundColor = color;
				}

				public void Dispose() {
					 GUI.backgroundColor = cache;
				}
		  }

		  public class ColorScope : IDisposable {
				private Color cache;

				public ColorScope(Color color) {
					 cache = GUI.color;
					 GUI.color = color;
				}

				public void Dispose() {
					 GUI.color = cache;
				}
		  }

		  public class ColorContentScope : IDisposable {
				private Color cache;

				public ColorContentScope(Color color) {
					 cache = GUI.contentColor;
					 GUI.contentColor = color;
				}

				public void Dispose() {
					 GUI.contentColor = cache;
				}
		  }

		  public static class Layout {

				public static void SallPropertyField(SerializedProperty property, string displayName = null, float size = 20) {
					 var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
					 if (string.IsNullOrEmpty(displayName)) {
						  displayName = property.displayName;
					 }
					 EditorGUI.LabelField(rect, displayName);
					 rect.x = rect.xMax - size;
					 rect.width = size;
					 EditorGUI.PropertyField(rect, property, GUIContent.none);
				}

				public static void InfoLabel(GUIContent label, object obj) {
					 var rect = GUILayoutUtility.GetRect(label, EditorStyles.miniLabel, new GUILayoutOption[0]);
					 GUIAddons.InfoLabel(rect, label, obj);
				}

				public static void InfoLabel(string label, object obj) {
					 InfoLabel(new GUIContent(label), obj);
				}

				public static void Table(params object[] values) {
					 var rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.miniLabel);
					 GUIAddons.Table(rect, values);
				}

				public static void StyledTable(GUIStyle style, params object[] values) {
					 var rect = GUILayoutUtility.GetRect(GUIContent.none, style);
					 GUIAddons.StyledTable(rect, style, values);
				}

				public static bool DrawOpenCloseButton(bool disableState, bool isOpen, GUIContent openContent, GUIContent closeContent) {
					 var click = false;
					 using (new EditorGUI.DisabledGroupScope(disableState)) {
						  GUIContent content = (isOpen) ? closeContent : openContent;
						  GUILayout.BeginHorizontal();
						  GUILayout.FlexibleSpace();
						  if (GUILayout.Button(content, EditorStyles.miniButton, drawOpenCloseButtonGUILayoutOption)) {
								click = true;
						  }
						  GUILayout.EndHorizontal();
					 }
					 return click;
				}
		  }
		  
		  public static bool DrawOpenCloseButton(Rect rect, bool disableState, bool isOpen, GUIContent openContent, GUIContent closeContent) {
				var click = false;
				using (new EditorGUI.DisabledGroupScope(disableState)) {
					 GUIContent content = (isOpen) ? closeContent : openContent;
					 rect.x = rect.xMax - 110;
					 rect.width = 110;
					 if (GUI.Button(rect, content, EditorStyles.miniButton)) {
						  click = true;
					 }
				}
				return click;
		  }
		  
		  public static void InfoLabel(Rect rect, GUIContent label, object obj) {
				var rectForValue = EditorGUI.PrefixLabel(rect, label, EditorStyles.miniLabel);
				EditorGUI.LabelField(rectForValue, obj.ToString(), EditorStyles.miniLabel);
		  }

		  public static void Table(Rect rect, params object[] values) {
				var rect_0 = new Rect(rect);
				rect_0.width /= values.Length;
				foreach (var value in values) {
					 EditorGUI.LabelField(rect_0, value.ToString(), EditorStyles.miniLabel);
					 rect_0.x = rect_0.xMax;
				}
		  }

		  public static void StyledTable(Rect rect, GUIStyle style, params object[] values) {
				var rect_0 = new Rect(rect);
				rect_0.width /= values.Length;
				foreach (var value in values) {
					 EditorGUI.LabelField(rect_0, value.ToString(), style);
					 rect_0.x = rect_0.xMax;
				}
		  }
		  /// <summary>
		  /// Remember repaint inspector to see hover hilight, use stopwatch pattern
		  /// </summary>
		  /// <param name="rect"></param>
		  /// <param name="content"></param>
		  /// <returns></returns>
		  public static bool HilightButton(Rect rect, GUIContent content) {
				using (new AWI.GUIAddons.ColorBackgroundScope(hilightColor)) {
					 return GUI.Button(rect, content, MiscStyles.easyHilight);
				}
		  }

		  public static readonly GUIStyle smallTickbox;
		  public static readonly GUIStyle miniLabelButton;

		  static readonly Color splitterDark;
		  static readonly Color splitterLight;
		  public static Color splitter { get { return EditorGUIUtility.isProSkin ? splitterDark : splitterLight; } }

		  static readonly Texture2D paneOptionsIconDark;
		  static readonly Texture2D paneOptionsIconLight;
		  public static Texture2D paneOptionsIcon { get { return EditorGUIUtility.isProSkin ? paneOptionsIconDark : paneOptionsIconLight; } }

		  public static readonly GUIStyle headerLabel;

		  static readonly Color headerBackgroundDark;
		  static readonly Color headerBackgroundLight;
		  public static Color headerBackground { get { return EditorGUIUtility.isProSkin ? headerBackgroundDark : headerBackgroundLight; } }

		  public static readonly GUIStyle wheelLabel;
		  public static readonly GUIStyle wheelThumb;
		  public static readonly Vector2 wheelThumbSize;

		  public static readonly GUIStyle preLabel;

		  public static void DrawCheckbox(Rect rect, GUIContent content, SerializedProperty property) {
				property.boolValue = GUI.Toggle(rect, property.boolValue, content, smallTickbox);
		  }

		  public static bool DrawExpandableHeader(SerializedProperty property, GUIContent tittle) {
				return property.isExpanded = DrawEspandableHeader(tittle, property.isExpanded);
		  }

		  public static bool DrawExpandableHeader(ref bool isExpanded, GUIContent tittle) {
				return isExpanded = DrawEspandableHeader(tittle, isExpanded);
		  }

		  private static bool DrawEspandableHeader(GUIContent tittle, bool state, int depth = -1) {
				var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

				var labelRect = backgroundRect;
				labelRect.xMin += 16f;
				labelRect.xMax -= 20f;

				labelRect.xMin += 16 * depth;
				labelRect.xMax -= 16 * depth;

				var foldoutRect = backgroundRect;
				foldoutRect.y += 1f;
				foldoutRect.width = 13f;
				foldoutRect.height = 13f;

				foldoutRect.xMin += 16 * depth;
				foldoutRect.xMax -= 16 * depth;

				// Background rect should be full-width
				backgroundRect.xMin = 0f;
				backgroundRect.width += 4f;

				// Background
				EditorGUI.DrawRect(backgroundRect, headerBackground);

				// Title
				EditorGUI.LabelField(labelRect, tittle, EditorStyles.boldLabel);

				// Foldout
				state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

				var e = Event.current;
				if (e.type == EventType.MouseDown && backgroundRect.Contains(e.mousePosition) && e.button == 0) {
					 state = !state;
					 e.Use();
				}

				return state;
		  }

		  static GUIAddons() {
				smallTickbox = new GUIStyle("ShurikenToggle");

				miniLabelButton = new GUIStyle(EditorStyles.miniLabel);
				miniLabelButton.normal = new GUIStyleState {
					 //background = RuntimeUtilities.transparentTexture,
					 scaledBackgrounds = null,
					 textColor = Color.grey
				};
				var activeState = new GUIStyleState {
					 //background = RuntimeUtilities.transparentTexture,
					 scaledBackgrounds = null,
					 textColor = Color.white
				};
				miniLabelButton.active = activeState;
				miniLabelButton.onNormal = activeState;
				miniLabelButton.onActive = activeState;

				splitterDark = new Color(0.12f, 0.12f, 0.12f, 1.333f);
				splitterLight = new Color(0.6f, 0.6f, 0.6f, 1.333f);

				headerBackgroundDark = new Color(0.1f, 0.1f, 0.1f, 0.2f);
				headerBackgroundLight = new Color(1f, 1f, 1f, 0.2f);

				paneOptionsIconDark = (Texture2D)EditorGUIUtility.Load("Builtin Skins/DarkSkin/Images/pane options.png");
				paneOptionsIconLight = (Texture2D)EditorGUIUtility.Load("Builtin Skins/LightSkin/Images/pane options.png");

				headerLabel = new GUIStyle(EditorStyles.miniLabel);

				wheelThumb = new GUIStyle("ColorPicker2DThumb");

				wheelThumbSize = new Vector2(
					 !Mathf.Approximately(wheelThumb.fixedWidth, 0f) ? wheelThumb.fixedWidth : wheelThumb.padding.horizontal,
					 !Mathf.Approximately(wheelThumb.fixedHeight, 0f) ? wheelThumb.fixedHeight : wheelThumb.padding.vertical
				);

				wheelLabel = new GUIStyle(EditorStyles.miniLabel);

				preLabel = new GUIStyle("ShurikenLabel");
		  }

		  public static void DrawSplitter() {
				var rect = GUILayoutUtility.GetRect(1f, 1f);

				// Splitter rect should be full-width
				rect.xMin = 0f;
				rect.width += 4f;

				if (Event.current.type != EventType.Repaint)
					 return;

				EditorGUI.DrawRect(rect, splitter);
		  }

		  public static void DrawBoolProperty(GUIContent content, SerializedProperty property) {
				using (new GUILayout.HorizontalScope()) {
					 var overrideRect = GUILayoutUtility.GetRect(17f, 17f, GUILayout.ExpandWidth(false));
					 overrideRect.yMin += 4f;
					 DrawCheckbox(overrideRect, GUIContent.none, property);
					 using (new EditorGUI.DisabledScope(!property.boolValue)) {
						  EditorGUILayout.LabelField(content);
					 }
				}
		  }

		  public static void DrawBoolProperty(string title, SerializedProperty property) {
				using (new GUILayout.HorizontalScope()) {
					 var overrideRect = GUILayoutUtility.GetRect(17f, 17f, GUILayout.ExpandWidth(false));
					 overrideRect.yMin += 4f;
					 DrawCheckbox(overrideRect, GUIContent.none, property);
					 using (new EditorGUI.DisabledScope(!property.boolValue)) {
						  EditorGUILayout.LabelField(title);
					 }
				}
		  }

		  public static void DrawBoolProperty(SerializedProperty property) {
				DrawBoolProperty(property.displayName, property);
		  }

	 }

	 public static class PropertyAddons {

		  public static Rect SingleLinePropertyField(Rect rect, SerializedProperty property) {
				EditorGUI.PropertyField(rect, property);
				rect.y = rect.yMax;
				return rect;
		  }

		  public static Rect DrawWithUnit(Rect rect, SerializedProperty property, string unit) {
				var cRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight };
				cRect.width -= 32;
				var rightRect = new Rect(rect);
				rightRect.x = cRect.xMax + 4;
				rightRect.width = 32 - 4;
				EditorGUI.PropertyField(cRect, property);
				GUI.Label(rightRect, unit);
				rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				return rect;
		  }

		  public static Rect DrawWithUnit(Rect rect, string label, string value, string unit) {
				var cRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight };
				cRect.width -= 32;
				var rightRect = new Rect(rect);
				rightRect.x = cRect.xMax + 4;
				rightRect.width = 32 - 4;
				var field = EditorGUI.PrefixLabel(cRect, new GUIContent(label));
				EditorGUI.SelectableLabel(field, value);
				GUI.Label(rightRect, unit);
				rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				return rect;
		  }

		  public static void RightToggleProperty(Rect rect, SerializedProperty property, string displayName = null, float size = 20) {
				if (string.IsNullOrEmpty(displayName)) {
					 displayName = property.displayName;
				}
				rect.y -= 3;
				EditorGUI.LabelField(rect, displayName, Labels.WhiteLabel(9, TextAnchor.MiddleLeft));
				rect.x = rect.xMax - size;
				rect.width = size;
				rect.y += 3;
				property.boolValue = GUI.Toggle(rect, property.boolValue, GUIContent.none, MiscStyles.shurikenToggle);
		  }

		  public static void SmartObjectField(Rect rect, SerializedProperty property, Func<Component, bool> condition) {
				EditorGUI.BeginChangeCheck();
				EditorGUI.PropertyField(rect, property, GUIContent.none);
				if (EditorGUI.EndChangeCheck()) {
					 SmartObjectFieldPopup(rect, property, property.objectReferenceValue, condition);
				}
		  }

		  public static void SmartGameObjectField(Rect rect, SerializedProperty property, Func<Component, bool> condition) {
				EditorGUI.BeginChangeCheck();
				EditorGUI.PropertyField(rect, property, GUIContent.none);
				if (EditorGUI.EndChangeCheck()) {
					 SmartGameObjectFieldPopup(rect, property, property.objectReferenceValue, condition);
				}
		  }

		  private static void SmartObjectFieldPopup(Rect rect, SerializedProperty property, UnityEngine.Object value, Func<Component, bool> condition) {
				if (value != null && value is Component) {
					 var menu = new GenericMenu();
					 var components = (value as Component).GetComponents<Component>();
					 foreach (var component in components) {
						  if (condition == null || condition(component)) {
								menu.AddItem(new GUIContent(component.GetType().Name), false, () => {
									 property.serializedObject.Update();
									 property.objectReferenceValue = component;
									 property.serializedObject.ApplyModifiedProperties();
								});
						  }
					 }
					 menu.DropDown(rect);
				}
		  }

		  private static void SmartGameObjectFieldPopup(Rect rect, SerializedProperty property, UnityEngine.Object value, Func<Component, bool> condition) {
				if (value != null && value is GameObject) {
					 var menu = new GenericMenu();
					 var components = (value as GameObject).GetComponents<Component>();
					 foreach (var component in components) {
						  if (condition == null || condition(component)) {
								menu.AddItem(new GUIContent(component.GetType().Name), false, () => {
									 property.serializedObject.Update();
									 property.objectReferenceValue = component;
									 property.serializedObject.ApplyModifiedProperties();
								});
						  }
					 }
					 menu.DropDown(rect);
				}
		  }

	 }

}
