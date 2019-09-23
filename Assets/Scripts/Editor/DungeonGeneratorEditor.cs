using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor {
	
	private DungeonGenerator _target;

	public void OnEnable() {
		_target = target as DungeonGenerator;
	}

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Generate")) 
			_target.Generate();
		
		if (GUILayout.Button("Deregenerate")) 
			_target.Deregenerate();
		
	}
}
