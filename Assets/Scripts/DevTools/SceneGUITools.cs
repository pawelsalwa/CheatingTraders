#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class SceneGUITools {

	private static readonly float SETTINGS_WIDTH = 300f;
	private static readonly float SETTINGS_HEIGHT = 300f;
	
	private static bool settingsShown = true;
	
	[RuntimeInitializeOnLoadMethod]
	[InitializeOnLoadMethod]
	private static void InitSceneTool () {
		SceneView.onSceneGUIDelegate += OnSceneGUI;
//		SceneView.duringSceneGui += OnSceneGUI;
	}
	
	private static void OnSceneGUI(SceneView sceneView) {
		Rect rect = new Rect(Screen.width - SETTINGS_WIDTH, Screen.height - SETTINGS_HEIGHT - 40, SETTINGS_WIDTH, SETTINGS_HEIGHT);
		Rect btnShowRect = new Rect(rect) { width = 60, height = 20, x = rect.x + rect.width - 60, y = rect.y - 20F };
		
		if (GUI.Button(btnShowRect, settingsShown ? "hide" : "show"))
			settingsShown = !settingsShown;

		if (settingsShown) {
			GUILayout.BeginArea(rect, new GUIStyle("HelpBox"));
			var xd = GUILayout.TextField("xddddddd");
			
			GM.projectConstants.combatDebugs = GUILayout.Toggle(GM.projectConstants.combatDebugs, "combatDebugs");
			GUILayout.EndArea();
		}
		
		SceneView.RepaintAll();
	}
		
}

#endif
