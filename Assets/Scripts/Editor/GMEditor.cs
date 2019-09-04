using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(GM))]
public class GMEditor : Editor {

    private GM _target;

    public void OnEnable() {
        _target = target as GM;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Spawn new enemy")) 
            _target.SpawnEnemy();
		
    }
}
#endif