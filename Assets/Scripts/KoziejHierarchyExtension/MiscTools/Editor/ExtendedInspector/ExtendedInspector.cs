using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace AWI {
	 [CanEditMultipleObjects]
	 [CustomEditor(typeof(MonoBehaviour), true)]
	 public class ExtendedInspector : Editor {
		  
		  public class ExtendedInspectorStyles {
				public static GUIContent[] previewToolbar = new GUIContent[] {
					 new GUIContent("Previous"),
					 new GUIContent("Next"),
				};

				public static GUIContent[] referenceToolbar = new GUIContent[] {
					 new GUIContent("C","Copy"),
					 new GUIContent("P","Paste"),
				};

				public static GUIContent[] arrayTools = new GUIContent[] {
					 new GUIContent("Move Up"),
					 new GUIContent("Move Down"),
					 new GUIContent("Delete Array Element"),
					 new GUIContent("Duplicate Array Element"),
				};

				public static float errorIconHalfWidth = 16;
				public GUIStyle footBackground;
				public GUIStyle elementBackground;
				public GUIStyle listHeader;
				public GUIStyle plus;
				public GUIStyle minus;

				public GUIStyle title;
				public GUIStyle error;
				public GUIStyle popup;
				public GUIStyle label;
				public GUIStyle unityEventLabel;
				public GUIStyle options;
				public Color headerColor;
				public Color backgroundColor;
				public Color controlColor;

				public RectOffset sizeRectOffset = new RectOffset(0, 0, -2, -2);
				public static float sizeRectWidth = 30;
				public static float sizeRectLeftSpace = 8;
				public static float refrenceToolbarWidth = 40;

				private static ExtendedInspectorStyles m_ProSkin;
				private static ExtendedInspectorStyles m_PersonalSkin;
				
				public static ExtendedInspectorStyles instance {
					 get {
						  if (EditorGUIUtility.isProSkin) {
								if (m_ProSkin == null) {
									 m_ProSkin = new ExtendedInspectorStyles();
								}
								return m_ProSkin;
						  } else {
								if (m_PersonalSkin == null) {
									 m_PersonalSkin = new ExtendedInspectorStyles();
								}
								return m_PersonalSkin;
						  }
					 }
				}

				private ExtendedInspectorStyles() {
					 this.footBackground = new GUIStyle("RL Background");
					 this.footBackground.stretchHeight = false;
					 this.listHeader = new GUIStyle("RL Header");
					 this.error = new GUIStyle("CN EntryError");
					 this.popup = new GUIStyle();
					 this.title = new GUIStyle(EditorStyles.miniLabel);
					 this.label = new GUIStyle("ShurikenEditableLabel");
					 this.options = new GUIStyle("Icon.TrackOptions");
					 this.options.margin = new RectOffset(2, 2, 6, 6);
					 this.unityEventLabel = new GUIStyle("ProfilerPaneSubLabel");
					 this.title.fixedHeight = 0;
					 this.label.normal.textColor = Color.white;
					 this.popup.normal.textColor = Color.white;
					 this.unityEventLabel.normal.textColor = Color.white;
					 InicializeReorderableStuff();
				}

				private void InicializeReorderableStuff() {
					 //hack to instantiate reorderable list default behaviours
					 var rl = new ReorderableList(new List<bool>(0), typeof(bool));
					 rl.DoList(new Rect(0, 0, 0, 0));
					 this.plus = new GUIStyle("ShurikenPlus");
					 this.minus = new GUIStyle("ShurikenMinus");
					 this.minus.fixedWidth = this.minus.fixedHeight = this.plus.fixedWidth = this.plus.fixedHeight = 16;
					 this.plus.normal.background = (Texture2D)ReorderableList.defaultBehaviours.iconToolbarPlus.image;
					 this.minus.normal.background = (Texture2D)ReorderableList.defaultBehaviours.iconToolbarMinus.image;
					 this.elementBackground = new GUIStyle(ReorderableList.defaultBehaviours.elementBackground);
					 this.elementBackground.border = new RectOffset(this.elementBackground.border.left, this.elementBackground.border.right, 10, 10);
				}

		  }

		  protected class ArrayPropertyInfo {
				public NiceListMode mode;
				public bool expanded = false;
				public bool usePreviewToEdit = false;
				public Vector2 scroll;
				public ReorderableList reorderableList;
				public ReorderableList.ElementCallbackDelegate elementCallbackDelegate;
				public ReorderableList.ElementCallbackDelegate emptyElementCallbackDelegate;
		  }

		  private const System.Reflection.BindingFlags fieldBindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

		  protected Dictionary<string, ArrayPropertyInfo> dict = new Dictionary<string, ArrayPropertyInfo>();
		  
		  private static UnityEngine.Object objectReference;
		  private static string stringType = "string";

		  private SerializedProperty focusedProperty = null;
		  private ReorderableList rl = null;
		  private bool useNiceListing = false;

		  public GUIContent GetTittleContent(SerializedProperty arrayProperty) {
				return new GUIContent(arrayProperty.displayName + " (" + arrayProperty.arrayElementType.Replace("PPtr<$", "").Replace(">", "") + ")");
		  }

		  public void DoReordelableList(SerializedProperty iterator, bool dontLeaveSpace, bool usePreviewToEdit, bool forceExpand) {
				var property = iterator.Copy();
				var info = null as ArrayPropertyInfo;
				if (!dontLeaveSpace) {
					 GUILayout.Space(5);
				}
				if(!dict.TryGetValue(property.propertyPath, out info)) {
					 info = new ArrayPropertyInfo();
					 info.reorderableList = new ReorderableList(serializedObject, property, true, true, true, true);
					 info.usePreviewToEdit = usePreviewToEdit;
					 info.elementCallbackDelegate = (rect, id, b, focused) => {
						  rect.x += 14;
						  rect.xMax -= 14;
						  var element = info.reorderableList.serializedProperty.GetArrayElementAtIndex(id);
						  if (focused) {
								this.rl = info.reorderableList;
								focusedProperty = element;
						  }
						  if (info.usePreviewToEdit) {
								EditorGUI.LabelField(rect, element.displayName);
								return;
						  }
						  if (element.propertyType == SerializedPropertyType.ObjectReference) {
								DrawObjectReferenceTools(ref rect, element);
						  }
						  var next = element.Copy();
						  var complexProperty = false;
						  while (next.NextVisible(false)) {
								if (!next.propertyPath.Contains(element.propertyPath)) {
									 break;
								}
								if (complexProperty == false) {
									 rect.y += EditorGUIUtility.singleLineHeight / 2;
									 complexProperty = true;
								}
								var h = EditorGUI.GetPropertyHeight(next, true);
								rect.height = h;
								EditorGUI.PropertyField(rect, next, true);
								rect.y += h + EditorGUIUtility.standardVerticalSpacing;
						  }
						  if (complexProperty == false) {
								rect.y += 2;
								EditorGUI.PropertyField(rect, element, true);
						  }
					 };
					 var tittleContent = GetTittleContent(property);
					 info.reorderableList.onRemoveCallback = (rl) => {
						  if (rl.count == 0 || rl.index == -1) {
								return;
						  }
						  ReorderableList.defaultBehaviours.DoRemoveButton(rl);
						  if (rl.count == 0) {
								focusedProperty = null;
						  }
					 };
					 info.reorderableList.drawHeaderCallback = (rect) => {
						  if (info.expanded) {
								var plusRect = new Rect(rect);
								plusRect.x = rect.xMax - 25f * 2;
								plusRect.width = 25f;
								plusRect.height = 13f;
								var removeRect = new Rect(plusRect);
								removeRect.x += 25;
								
								var add = GUI.Button(plusRect, ReorderableList.defaultBehaviours.iconToolbarPlus, ReorderableList.defaultBehaviours.preButton);
								var remove = GUI.Button(removeRect, ReorderableList.defaultBehaviours.iconToolbarMinus, ReorderableList.defaultBehaviours.preButton);

								if (add) {
									 if (info.reorderableList.onAddCallback != null) {
										  info.reorderableList.onAddCallback(info.reorderableList);
									 } else {
										  ReorderableList.defaultBehaviours.DoAddButton(info.reorderableList);
									 }
								}

								if (remove) {
									 if (info.reorderableList.onRemoveCallback != null) {
										  info.reorderableList.onRemoveCallback(info.reorderableList);
									 } else {
										  ReorderableList.defaultBehaviours.DoRemoveButton(info.reorderableList);
									 }
								}

								if (GUI.Button(rect, tittleContent, ExtendedInspectorStyles.instance.title)) {
									 if (!forceExpand) {
										  info.expanded = !info.expanded;
									 }
								}
						  } else {
								if (GUI.Button(rect, tittleContent, ExtendedInspectorStyles.instance.title)) {
									 if (!forceExpand) {
										  info.expanded = !info.expanded;
									 }
								}
						  }
					 };
					 info.reorderableList.elementHeightCallback = (index) => {
						  if (property.isExpanded) {
								if (info.usePreviewToEdit) {
									 return EditorGUIUtility.singleLineHeight;
								}else {
									 var h = EditorGUI.GetPropertyHeight(info.reorderableList.serializedProperty.GetArrayElementAtIndex(index));
									 h = h > 32 ? h + 8 : h + 5;
									 return h;
								}
						  } else {
								return 0;
						  }
					 };
					 info.reorderableList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
						  if (index < property.arraySize && index >= 0) {
								if (!info.usePreviewToEdit) {
									 var h = EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(index));
									 h = h > 32 ? h + 8 : h + 5;
									 rect.height = h;
								}
								if (Event.current.type == EventType.Repaint && property.isExpanded) {
									 ExtendedInspectorStyles.instance.elementBackground.Draw(rect, false, isActive, isActive, isFocused);
								}
						  }
					 };
					 info.reorderableList.drawFooterCallback = (Rect rect) => {
					 };
					 info.expanded = property.isExpanded || forceExpand;
					 info.usePreviewToEdit = usePreviewToEdit;
					 dict.Add(iterator.propertyPath, info);
				}
				if (forceExpand) {
					 info.expanded = true;
				}
				var scroll = false;
				if (info.reorderableList.count > 5) {
					 scroll = false;
				}
				if (scroll) {
					 info.scroll = GUILayout.BeginScrollView(info.scroll, GUILayout.Height(100));
				}
				if (info.expanded) {
					 info.reorderableList.drawElementCallback = info.elementCallbackDelegate;
					 info.reorderableList.draggable = true;
					 info.reorderableList.displayAdd = false;
					 info.reorderableList.displayRemove = false;
					 info.reorderableList.footerHeight = 0;
					 if (property.arraySize == 0) {
						  info.reorderableList.elementHeight = 16;
					 }
				} else {
					 info.reorderableList.drawElementCallback = info.emptyElementCallbackDelegate;
					 info.reorderableList.elementHeight = 0;
					 info.reorderableList.draggable = false;
					 info.reorderableList.displayAdd = false;
					 info.reorderableList.displayRemove = false;
					 info.reorderableList.footerHeight = 0;
					 info.reorderableList.index = -1;
				}
				info.reorderableList.DoLayoutList();
				iterator.isExpanded = info.expanded;
				if (scroll) {
					 GUILayout.EndScrollView();
				}	
		  }

		  protected virtual void OnEnable() {
				if(target == null) {
					 return;
				}
				var objects = (target.GetType().GetCustomAttributes(true));
				foreach(var obj in objects) {
					 if(obj is UseNiceListingAttribute) {
						  useNiceListing = true;
					 }
				}
		  }

		  public override void OnInspectorGUI() {
				if (!useNiceListing) {
					 base.OnInspectorGUI();
					 return;
				}
				serializedObject.Update();
				SerializedProperty iterator = serializedObject.GetIterator();
				bool enterChildren = true;
				var skipSpace = true;
				GUILayout.Space(2);
				while (iterator.NextVisible(enterChildren)) {
					 bool drawArray = iterator.isArray && iterator.type != stringType;
					 if (drawArray) {
						  GUILayout.Space(2);
						  OnDrawArray(iterator, skipSpace);
					 }
					 skipSpace = drawArray;
					 if (!drawArray) {
						  EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
					 }
					 enterChildren = false;
				}
				serializedObject.ApplyModifiedProperties();
		  }

		  public override bool HasPreviewGUI() {
				return true;
		  }

		  public override void DrawPreview(Rect previewArea) {
				serializedObject.Update();
				if (rl != null) {
					 var toolbarRect = new Rect(previewArea);
					 toolbarRect.height = EditorGUIUtility.singleLineHeight;
					 var option = GUI.Toolbar(toolbarRect, -1, ExtendedInspectorStyles.previewToolbar, AWI.SceneViewTools.Layout.button);
					 if (focusedProperty != null) {
						  previewArea.y += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
						  //focusedProperty.isExpanded = true;
						  EditorGUI.PropertyField(previewArea, focusedProperty, true);
					 }
					 if (option == 0) {
						  rl.index = (rl.index + rl.count - 1) % rl.count;
						  focusedProperty = rl.serializedProperty.GetArrayElementAtIndex(rl.index);
					 }
					 if (option == 1) {
						  rl.index = (rl.index + 1) % rl.count;
						  focusedProperty = rl.serializedProperty.GetArrayElementAtIndex(rl.index);
					 }
				}
				serializedObject.ApplyModifiedProperties();
		  }
		  
		  private Dictionary<Type, Dictionary<string, NiceListAttribute>> attributeInfoCache = new Dictionary<Type, Dictionary<string, NiceListAttribute>>();
		  
		  private NiceListAttribute GetAttributeInfo(Type type, string propertyPath) {
				var inDict = null as Dictionary<string, NiceListAttribute>;
				if (!attributeInfoCache.TryGetValue(type, out inDict)) {
					 inDict = new Dictionary<string, NiceListAttribute>();
				}
				var attribute = null as NiceListAttribute;
				if (!inDict.TryGetValue(propertyPath, out attribute)) {
					 var field = type.GetField(propertyPath, fieldBindingFlags);
					 if(field == null) {
						  return NiceListAttribute.defaultAttribute;
					 }
					 var attributes = field.GetCustomAttributes(typeof(NiceListAttribute), false);
					 if (attributes != null && attributes.Length > 0) {
						  attribute = attributes[0] as NiceListAttribute;
					 }else {
						  attribute = NiceListAttribute.defaultAttribute;
					 }
					 inDict.Add(propertyPath, attribute);
				}
				return attribute;
		  }

		  private int DrawSizeArray(SerializedProperty property, Rect labelRect) {
				var prefixSizeRect = new Rect(labelRect) { x = labelRect.xMax, width = 15 };
				var sizeRect = new Rect(labelRect) { width = 25, x = prefixSizeRect.xMax };
				EditorGUI.LabelField(prefixSizeRect, new GUIContent("#"), EditorStyles.boldLabel);
				if (property.isExpanded) {
					 EditorGUI.BeginChangeCheck();
					 var value = 0;
					 using (new GUIAddons.ColorBackgroundScope(new Color(1, 1, 1, 0.75f))) {
						  value = EditorGUI.DelayedIntField(sizeRect, GUIContent.none, property.arraySize);
					 }
					 if (!EditorGUI.EndChangeCheck()) {
						  return -1;
					 } else {
						  return value;
					 }
				} else {
					 EditorGUI.LabelField(sizeRect, new GUIContent(property.arraySize.ToString()), EditorStyles.boldLabel);
					 return -1;
				}
		  }

		  private void OnDrawArray(SerializedProperty iterator, bool dontLeaveSpace) {
				NiceListAttribute attribute = GetAttributeInfo(target.GetType(), iterator.propertyPath);
				iterator.isExpanded = iterator.isExpanded | attribute.forceExpand;
				if (attribute.mode == NiceListMode.Reorderable) {
					 DoReordelableList(iterator, dontLeaveSpace, attribute.compact, attribute.forceExpand);
				} else if(attribute.mode != NiceListMode.Legacy) {
					 GUILayout.Label("", ExtendedInspectorStyles.instance.listHeader, GUILayout.ExpandWidth(true));
					 var labelRect = GUILayoutUtility.GetLastRect();
					 labelRect.width -= 56;
					 labelRect.x += 6;
					 iterator.isExpanded = GUI.Toggle(labelRect, iterator.isExpanded | attribute.forceExpand, GetTittleContent(iterator), ExtendedInspectorStyles.instance.title);
					 var resizeValue = DrawSizeArray(iterator, labelRect);
					 if (iterator.isExpanded) {
						  using (new GUILayout.VerticalScope(ExtendedInspectorStyles.instance.footBackground)) {
								int index = 0;
								bool first = true;
								bool popupMenu = false;
								var arrayIterator = iterator.Copy();
								string arrayPath = iterator.propertyPath + ".";
								using (new GUIAddons.LabelWdithScope(60)) {
									 while (arrayIterator.NextVisible(first) && arrayIterator.propertyPath.Contains(arrayPath)) {
										  if (!first) {
												GUILayout.BeginHorizontal();
												popupMenu = GUILayout.Button("", ExtendedInspectorStyles.instance.options, GUILayout.Width(16), GUILayout.Height(16));
												if (popupMenu) {
													 PopupElementMenu(iterator.Copy(), index);
												}
												EditorGUI.indentLevel++;
												if (arrayIterator.hasChildren) {
													 EditorGUILayout.PropertyField(arrayIterator, true, new GUILayoutOption[0]);
												} else {
													 EditorGUILayout.PropertyField(arrayIterator, new GUIContent(index.ToString()), true, new GUILayoutOption[0]);
												}
												EditorGUI.indentLevel--;
												if (arrayIterator.propertyType == SerializedPropertyType.ObjectReference) {
													 GUILayout.Space(2);
													 DrawObjectReferenceToolsLayout(arrayIterator);
												}
												GUILayout.EndHorizontal();
												index++;
										  }
										  first = false;
									 }
								}
								if (iterator.arraySize == 0) {
									 GUILayout.Label("List is Empty", EditorStyles.label);
								}
								GUILayout.Space(4);
						  }
					 } else {
						  using (new GUILayout.VerticalScope(ExtendedInspectorStyles.instance.footBackground)) {
								GUILayout.Space(8);
						  }
					 }
					 if (resizeValue != -1) {
						  iterator.arraySize = resizeValue;
					 }
				} else {
					 EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
				}
		  }

		  private static void PopupElementMenu(SerializedProperty array, int index) {
				var menu = new GenericMenu();
				menu.AddItem(ExtendedInspectorStyles.arrayTools[0], false, () => {
					 array.serializedObject.Update();
					 array.MoveArrayElement((index - 1 + array.arraySize) % array.arraySize, index);
					 array.serializedObject.ApplyModifiedProperties();
				});
				menu.AddItem(ExtendedInspectorStyles.arrayTools[1], false, () => {
					 array.serializedObject.Update();
					 array.MoveArrayElement((index + 1) % array.arraySize, index);
					 array.serializedObject.ApplyModifiedProperties();
				});
				menu.AddItem(ExtendedInspectorStyles.arrayTools[2], false, () => {
					 array.serializedObject.Update();
					 array.GetArrayElementAtIndex(index).DeleteCommand();		
					 array.serializedObject.ApplyModifiedProperties();
				});
				menu.AddItem(ExtendedInspectorStyles.arrayTools[3], false, () => {
					 array.serializedObject.Update();
					 array.GetArrayElementAtIndex(index).DuplicateCommand();
					 array.serializedObject.ApplyModifiedProperties();
				});
				menu.ShowAsContext();
		  }

		  private void DrawObjectReferenceTools(ref Rect rect, SerializedProperty property) {
				rect.width -= ExtendedInspectorStyles.refrenceToolbarWidth;
				var toolbarRect = new Rect(rect) { y = rect.y + 1, x = rect.xMax, width = ExtendedInspectorStyles.refrenceToolbarWidth, height = Mathf.Min(16, rect.height - 4) };
				rect.width -= 4;
				var tool = GUI.Toolbar(toolbarRect, -1, ExtendedInspectorStyles.referenceToolbar, EditorStyles.miniButton);
				if (tool == 0) {
					 objectReference = property.objectReferenceValue;
				} else if ((tool == 1) && objectReference != null) {
					 try {
						  property.objectReferenceValue = objectReference;
					 } catch {
					 }
				}
		  }

		  private void DrawObjectReferenceToolsLayout(SerializedProperty property) {
				var tool = GUILayout.Toolbar(-1, ExtendedInspectorStyles.referenceToolbar, EditorStyles.miniButton, GUILayout.Width(ExtendedInspectorStyles.refrenceToolbarWidth));
				if (tool == 0) {
					 objectReference = property.objectReferenceValue;
				} else if ((tool == 1) && objectReference != null) {
					 try {
						  property.objectReferenceValue = objectReference;
					 } catch {
					 }
				}
		  }

	 }

}
