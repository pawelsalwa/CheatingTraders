using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class DevTools {

	public enum DebugColor { red, green, blue, orange, magenta, white, gray }

	// usage: 
	// DevTools.Print("xddd", gameObject, DebugColor.green);
	// DevTools.Print("xddd", "g");

	public static void Print(string msg, GameObject obj = null, DebugColor debugCol = DebugColor.gray) {
		Debug.Log($"<color={debugCol}>{msg}</color>", obj);
	}

	public static void Print(string msg, DebugColor debugCol = DebugColor.gray) {
		Print(msg, null, debugCol);
	}
	
	public static void Print(string msg, string col, GameObject obj = null) {
		DebugColor debugColor;
		switch (col) {
			case "r": debugColor = DebugColor.red; break;
			case "g": debugColor = DebugColor.green; break;
			case "b": debugColor = DebugColor.blue; break;
			case "o": debugColor = DebugColor.orange; break;
			case "m": debugColor = DebugColor.magenta; break;
			case "w": debugColor = DebugColor.white; break;
			default: debugColor = DebugColor.gray; break;
		};
		Print(msg, obj, debugColor);
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