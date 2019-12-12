using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class DevTools {

	public enum DebugColor { red, green, blue, orange, magenta, white, gray }

	// usage: 
	// Tools.Print("xddd", gameObject, DebugColor.green);

	public static void Print(string msg, GameObject obj = null, DebugColor debugCol = DebugColor.gray) {
		Debug.Log($"<color={debugCol}>{msg}</color>", obj);
	}

	public static void Print(string msg, DebugColor debugCol = DebugColor.gray) {
		Print(msg, null, debugCol);
	}

	public static void CleanUnityConsole() {
#if UNITY_EDITOR
		var assembly = Assembly.GetAssembly(typeof(SceneView));
		var type = assembly.GetType("UnityEditor.LogEntries");
		var method = type.GetMethod("Clear");
		method.Invoke(new object(), null);
#endif
	}

}