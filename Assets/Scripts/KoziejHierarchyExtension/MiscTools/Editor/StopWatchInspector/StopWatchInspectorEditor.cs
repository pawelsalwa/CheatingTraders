using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AWI {
	 [CustomPropertyDrawer(typeof(StopWatchInspector))]
	 public class StopWatchInspectorEditor : PropertyDrawer {
		  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
				return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 4;
		  }

		  private string[] grid = new string[] {
				"fps", "0", "ms", "0", "ticks", "0"
		  };

		  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
				var swi = fieldInfo.GetValue(property.serializedObject.targetObject) as StopWatchInspector;
				position.y += EditorGUIUtility.standardVerticalSpacing;
				position.height -= EditorGUIUtility.standardVerticalSpacing * 2;
				if(Event.current.type == EventType.Repaint) {
					 EditorStyles.helpBox.Draw(position, GUIContent.none, -1);
				}
				position.width -= 8;
				position.x += 4;
				position.height = EditorGUIUtility.singleLineHeight;
				EditorGUI.LabelField(position, label, EditorStyles.centeredGreyMiniLabel);
				position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
				if (swi != null) {
					 grid[1] = swi.fps.ToString();
					 grid[3] = swi.miliseconds.ToString();
					 grid[5] = swi.tickets.ToString();
					 GUI.SelectionGrid(position, -1, grid, 6, EditorStyles.miniLabel);
				}
		  }
	 };
}// end of namespace
