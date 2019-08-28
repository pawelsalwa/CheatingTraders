using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AWI {
	 public static class PropertyUtilities {
		  public static class Layout {
				public static void PropertyFieldWithSkipLabel(SerializedProperty property, bool addIntendedLevel = true, bool forceExpand = false) {
					 if (addIntendedLevel) {
						  EditorGUI.indentLevel++;
					 }
					 var copy = property.Copy();
					 copy.isExpanded = true;
					 var end = copy.GetEndProperty();
					 while (copy.NextVisible(copy.isExpanded)) {
						  if (SerializedProperty.EqualContents(copy, end)) {
								break;
						  }
						  EditorGUILayout.PropertyField(copy, false);
						  if (forceExpand) {
								copy.isExpanded = true;
						  }
					 }
					 if (addIntendedLevel) {
						  EditorGUI.indentLevel--;
					 }
				}
		  };
	 };
}// end of namespace
