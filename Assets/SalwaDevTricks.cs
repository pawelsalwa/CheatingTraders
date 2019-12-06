using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Reflection;


public class SalwaDevTricks : MonoBehaviour {


    private void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
    }
}