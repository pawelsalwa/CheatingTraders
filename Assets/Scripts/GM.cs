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
[RequireComponent(typeof(ProjectConstants))]
public class GM : MonoBehaviour {

	public static event Action<BasicUnit> OnPlayerSpawned = player => { };

	private static GM _instance;
	public static GM instance => _instance == null ? _instance = FindObjectOfType<GM>() : _instance;

	public static bool isMultiplayer = false; 
	[Range(0f, 5f)]
	public float timeScale = 1;

	public CinemachineFreeLook cinemachineFreeLook;
	public Camera mainCamera;
	public BasicUnit basicUnitPrefab;
	public DungeonGenerator dungeonGenerator;

	public BasicUnit _player;
	public static BasicUnit player => instance._player == null ? null : instance._player.isAlive ? instance._player : null;
	
	private ProjectConstants _projectConstants;
	public static ProjectConstants projectConstants => instance._projectConstants == null ? instance._projectConstants = instance.GetComponent<ProjectConstants>() : instance._projectConstants;
			
	private static bool _isGamePaused;
	public static bool isGamePaused {
		private set {
			_isGamePaused = value;
			Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
			instance.cinemachineFreeLook.enabled = !value;
		}
		get => _isGamePaused;
	}

	public List<BasicUnit> aiUnits = new List<BasicUnit>();

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

	private void SpawnPlayer() {
		_player = Instantiate(basicUnitPrefab);
		_player.GetComponent<CharacterController>().enabled = false;
		_player.transform.position = dungeonGenerator.GetPlayerStartingPosition();
		_player.GetComponent<CharacterController>().enabled = true;
		_player.InitAsPlayer();
		instance.cinemachineFreeLook.enabled = true;

		OnPlayerSpawned(_player);
	}

	public void SpawnEnemy() {
		var newEnemy = Instantiate(basicUnitPrefab);
		aiUnits.Add(newEnemy);
		newEnemy.GetComponent<CharacterController>().enabled = false;
		newEnemy.transform.position = dungeonGenerator.GetPlayerStartingPosition() + Vector3.forward * Random.Range(0, 2);
		newEnemy.GetComponent<CharacterController>().enabled = true;
		newEnemy.InitAsBot();
		newEnemy.OnDeath += () => RemoveBot(newEnemy);
	}

	private void DestroyBots() {
		foreach (var unit in aiUnits) Destroy(unit.gameObject);
		aiUnits.Clear();
	}

	private void RemoveBot(BasicUnit bot) {
		if (aiUnits.Contains(bot))
			aiUnits.Remove(bot);
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.H)) DestroyBots();
		if (Input.GetKeyDown(KeyCode.G)) SpawnEnemy();
		if (Input.GetKeyDown(KeyCode.Y)) DeactivateBots();

		Time.timeScale = timeScale;
	}

	private void DeactivateBots() {
		foreach (var xd in aiUnits) 
			xd.GetComponent<BotController>().enabled = !xd.GetComponent<BotController>().enabled;
		
		foreach (var xd in aiUnits) 
			xd.GetComponent<Animator>().enabled = !xd.GetComponent<Animator>().enabled;
	}
}