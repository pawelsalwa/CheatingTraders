using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

///<summary> Game Manager and singleton </summary>
public class GM : NetworkBehaviour {

	private static GM _instance;
	public static GM instance => _instance == null ? _instance = FindObjectOfType<GM>() : _instance;

	public static bool isAnyMenuOpened => instance.allMenus.Any(x => x.isOpened);
	public static bool isMultiplayer = false;

	public CinemachineFreeLook cinemachineFreeLook;
	public Camera mainCamera;
	public BasicUnit basicUnitPrefab;

	public InGameMenu inGameMenu;
	public MainGameMenu mainGameMenu;
	public MultiplayerMenu multiplayerMenu;
	public YouDiedMenu youDiedMenu;

	private List<UIMenuPanelBase> allMenus => new List<UIMenuPanelBase> {inGameMenu, mainGameMenu, multiplayerMenu, youDiedMenu};

	public BasicUnit _player;
	public static BasicUnit player => instance._player.isAlive ? instance._player : null;

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

	private List<BasicUnit> AiUnits = new List<BasicUnit>();

	[RuntimeInitializeOnLoadMethod]
	private static void MakeSureInstanceIsActive() {
		instance.gameObject.SetActive(true);
	}

	private void Awake() {
		foreach (var rootObject in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
			foreach (var xd in rootObject.GetComponentsInChildren<Initable>(true))
				xd.Init();

		UIMenuPanelBase.OnAnyPanelClosed += () => {
			isGamePaused = isAnyMenuOpened;
		};
		UIMenuPanelBase.OnAnyPanelOpened += () => {
			isGamePaused = isAnyMenuOpened;
		};
		mainGameMenu.Open();
	}

	public static void EnableCamera(bool enabled) {
		instance.cinemachineFreeLook.enabled = enabled;
	}

	public void StartSinglePlayerGame() {
		isMultiplayer = false;
		SpawnPlayer();
		SpawnEnemy();
	}

	public void EndGame() {
		DestroyBots();
		if (isMultiplayer) {
			multiplayerMenu.StopMultiplayer();
		} else {
			if (player != null && player.isAlive)
				Destroy(player.gameObject);
		}
	}

	private void SpawnPlayer() {
		_player = Instantiate(basicUnitPrefab);
		_player.transform.position = Vector3.up;
		_player.InitAsPlayer();
		instance.cinemachineFreeLook.enabled = true;
		_player.OnDeath += youDiedMenu.Open;

		foreach (var go in _player.GetComponentsInChildren<Transform>()) go.tag = "Player";
//        foreach (var go in _player.GetComponentsInChildren<Transform>()) go.gameObject.layer = 1 >> 9;
	}

	public void SpawnEnemy() {
		var newEnemy = Instantiate(basicUnitPrefab);
		AiUnits.Add(newEnemy);
		newEnemy.userInputHandler.enabled = false;
		newEnemy.GetComponent<BotController>().enabled = true;
		newEnemy.transform.position = Vector3.up + new Vector3(Random.Range(0, 11), Random.Range(0, 11), Random.Range(0, 11));
		newEnemy.InitAsBot();
		newEnemy.OnDeath += () => RemoveBot(newEnemy);
		foreach (var go in newEnemy.GetComponentsInChildren<Transform>()) go.tag = "Enemy";
	}

	public static void OpenInGameMenu() {
		instance.inGameMenu.Open();
	}

	private void DestroyBots() {
		foreach (var unit in AiUnits) Destroy(unit.gameObject);
		AiUnits.Clear();
	}

	private void RemoveBot(BasicUnit bot) {
		if (AiUnits.Contains(bot))
			AiUnits.Remove(bot);
	}
}