using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

///<summary> Game Manager singleton </summary>
public class GM : NetworkBehaviour {

	private static GM _instance;
	public static GM instance => _instance == null ? _instance = FindObjectOfType<GM>() : _instance;

	public static bool isMultiplayer = false;

	public CinemachineFreeLook cinemachineFreeLook;
	public Camera mainCamera;
	public BasicUnit basicUnitPrefab;

	public DungeonGenerator dungeonGenerator;

	public BasicUnit _player;
	public static BasicUnit player => instance._player == null ? null : instance._player.isAlive ? instance._player : null;
	
	private ProjectConstants _projectConstants;

	public static ProjectConstants projectConstants {
		get {
			if (instance._projectConstants == null)
				instance._projectConstants = instance.GetComponent<ProjectConstants>();

			if (instance._projectConstants == null)
				instance._projectConstants = instance.gameObject.AddComponent<ProjectConstants>();

			return instance._projectConstants;
		}
	}
			
	private static bool _isGamePaused;

	public static bool isGamePaused {
		private set {
			_isGamePaused = value;
			Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
			instance.cinemachineFreeLook.enabled = !value;
		}
		get {
			return _isGamePaused;
		}
	}

	public List<BasicUnit> AiUnits = new List<BasicUnit>();

	[RuntimeInitializeOnLoadMethod]
	private static void MakeSureInstanceIsActive() {
		instance.gameObject.SetActive(true);
	}

	private void Awake() {
//		foreach (var rootObject in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
		foreach (var initable in GetComponentsInChildren<Initable>(true))
			initable.Init();

		UIMenuPanelBase.OnAnyPanelClosed += () => { isGamePaused = UIManager.isAnyMenuOpened; };
		UIMenuPanelBase.OnAnyPanelOpened += () => { isGamePaused = UIManager.isAnyMenuOpened; };
	}

	public static void EnableCamera(bool enabled) {
		instance.cinemachineFreeLook.enabled = enabled;
	}

	public void StartSinglePlayerGame() {
		isMultiplayer = false;

		dungeonGenerator?.Generate();
		SpawnPlayer();
		SpawnEnemy();
//		SpawnEnemy();
		//SpawnEnemy();
	}

	public void EndGame() {
		DestroyBots();
		if (isMultiplayer) {
			UIManager.instance.multiplayerMenu.StopMultiplayer();
		} else {
			if (player != null && player.isAlive)
				Destroy(player.gameObject);
		}
	}

	private Vector3 pos;

	private void SpawnPlayer() {
		_player = Instantiate(basicUnitPrefab);
		pos = dungeonGenerator.GetPlayerStartingPosition();
		_player.transform.position = dungeonGenerator.GetPlayerStartingPosition();
		_player.InitAsPlayer();
		instance.cinemachineFreeLook.enabled = true;

		foreach (var go in _player.GetComponentsInChildren<Transform>()) go.tag = "Player";
//        foreach (var go in _player.GetComponentsInChildren<Transform>()) go.gameObject.layer = 1 >> 9;
	}

	public void SpawnEnemy() {
		var newEnemy = Instantiate(basicUnitPrefab);
		AiUnits.Add(newEnemy);
		newEnemy.userInputHandler.enabled = false;
		newEnemy.GetComponent<BotController>().enabled = true;
		newEnemy.transform.position = dungeonGenerator.GetPlayerStartingPosition() + Vector3.forward * Random.Range(0, 2);
		newEnemy.InitAsBot();
		newEnemy.OnDeath += () => RemoveBot(newEnemy);
		foreach (var go in newEnemy.GetComponentsInChildren<Transform>()) go.tag = "Enemy";
	}

	private void DestroyBots() {
		foreach (var unit in AiUnits) Destroy(unit.gameObject);
		AiUnits.Clear();
	}

	private void RemoveBot(BasicUnit bot) {
		if (AiUnits.Contains(bot))
			AiUnits.Remove(bot);
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.H)) DestroyBots();
	}
	
	
}