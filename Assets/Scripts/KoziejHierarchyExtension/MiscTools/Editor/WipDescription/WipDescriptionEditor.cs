using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WipDescription))]
public class WipDescriptionEditor : Editor
{
	 SerializedProperty description;
	 private void OnEnable() {
		  description = serializedObject.FindProperty("description");
	 }
	 public override void OnInspectorGUI() {
		  serializedObject.Update();
		  GUILayout.Space(5);
		  EditorGUI.indentLevel--;
		  description.stringValue = GUILayout.TextArea(description.stringValue, GUILayout.Height(200));
		  EditorGUI.indentLevel++;
		  serializedObject.ApplyModifiedProperties();
	 }
}
