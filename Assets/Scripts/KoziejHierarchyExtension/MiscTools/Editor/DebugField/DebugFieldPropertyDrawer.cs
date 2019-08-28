using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AWI {
	 [CustomPropertyDrawer(typeof(DebugField), true)]
	 public class DebugFieldPropertyDrawer : PropertyDrawer {
		  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
				return EditorGUIUtility.singleLineHeight;
		  }
		  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
				var type = fieldInfo.FieldType;
				var field = fieldInfo.GetValue(property.serializedObject.targetObject);
				if (type == typeof(DebugFieldFloat)) {
					 var castedField = field as DebugFieldFloat;
					 position = EditorGUI.PrefixLabel(position, new GUIContent(property.displayName), Labels.leftGreyMiniLabel);
					 EditorGUI.SelectableLabel(position, castedField.value.ToString("F4"), Labels.leftGreyMiniLabel);
				} else if(type == typeof(DebugFieldBool)){
					 var castedField = field as DebugFieldBool;
					 position = EditorGUI.PrefixLabel(position, new GUIContent(property.displayName), Labels.leftGreyMiniLabel);
					 EditorGUI.SelectableLabel(position, castedField.value.ToString(), Labels.leftGreyMiniLabel);
				} else if (type == typeof(DebugFieldString)) {
					 var castedField = field as DebugFieldString;
					 position = EditorGUI.PrefixLabel(position, new GUIContent(property.displayName), Labels.leftGreyMiniLabel);
					 EditorGUI.SelectableLabel(position, castedField.value, Labels.leftGreyMiniLabel);
				}
		  }
	 }
}
