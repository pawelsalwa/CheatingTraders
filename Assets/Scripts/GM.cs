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
	public Camera camera;
	public BasicUnitNetwork _player;
	public static BasicUnitNetwork player => instance._player;

	public bool isMenuOpened => gameMenu.isOpened;

	[RuntimeInitializeOnLoadMethod]
	private static void InitAllInitables() {
		foreach (var rootObject in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()) 
			foreach (var xd in rootObject.GetComponentsInChildren<Initable>(true)) 
				xd.Init();
	}

	public static void EnableCamera(bool enabled) {
		instance.cinemachineFreeLook.enabled = enabled;
	}
}