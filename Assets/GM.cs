using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

///<summary> Game Manager and singleton </summary>
public class GM : MonoBehaviour {

	private static GM _instance;
	public static GM instance => _instance == null ? _instance = FindObjectOfType<GM>() : _instance;

	public GameMenu gameMenu;
	public CinemachineFreeLook cinemachineFreeLook;

	public bool isMenuOpened => gameMenu.isOpened;

	private void Awake() {
		foreach (var rootObject in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()) {
			var asd = rootObject.GetComponentsInChildren<Initable>(true);
			foreach (var xd in asd) {
				xd.Init();
			}
		}
	}

	public static void EnableCamera(bool enabled) {
		instance.cinemachineFreeLook.enabled = enabled;
	}
}