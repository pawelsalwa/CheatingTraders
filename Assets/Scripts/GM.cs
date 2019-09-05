using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

///<summary> Game Manager and singleton </summary>
public class GM : MonoBehaviour {

	private static GM _instance;
	public static GM instance => _instance == null ? _instance = FindObjectOfType<GM>() : _instance;

	public GameMenu gameMenu;
	public CinemachineFreeLook cinemachineFreeLook;
	public Camera mainCamera;
	public BasicUnit basicUnitPrefab;

	public bool isNetworkDevxD = false;

	private BasicUnit _player;
	public static BasicUnit player => instance._player;

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

	private void Start() {
		if (!isNetworkDevxD) {
			StartGame();
			Debug.Log("NIE MA NETA");
		} else 
			Debug.Log("Networkujemy");
	}

	private void StartGame() {
		SpawnPlayer();
		SpawnEnemy();
//		SpawnEnemy();
	}

	public void SpawnPlayer() {
		_player = Instantiate(basicUnitPrefab);
		_player.transform.position = Vector3.up;
		
		_player.userInputHandler.enabled = true;
		_player.GetComponent<BotController>().enabled = false;
		
		cinemachineFreeLook.m_Follow = _player.userInputHandler.cameraFollow;
		cinemachineFreeLook.m_LookAt = _player.userInputHandler.cameraOrbit;
		
        foreach (var go in _player.GetComponentsInChildren<Transform>()) go.tag = "Player";
//        foreach (var go in _player.GetComponentsInChildren<Transform>()) go.gameObject.layer = 1 >> 9;
	}

	public void SpawnEnemy() {
		var newEnemy = Instantiate(basicUnitPrefab);
		newEnemy.userInputHandler.enabled = false;
		newEnemy.GetComponent<BotController>().enabled = true;
		newEnemy.transform.position = Vector3.up + new Vector3(Random.Range(0, 6), Random.Range(0,6), Random.Range(0, 6));
		foreach (var go in newEnemy.GetComponentsInChildren<Transform>()) go.tag = "Enemy";
	}
}