using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// summary> singleton for UI </summary>
public class UIManager : MonoBehaviour {
	
	private static UIManager _instance;
	public static UIManager instance => _instance == null ? _instance = FindObjectOfType<UIManager>() : _instance;
	public static bool isAnyMenuOpened => instance.allMenus.Any(x => x.isOpened);

	public InGameMenu inGameMenu;
	public MainGameMenu mainGameMenu;
	public MultiplayerMenu multiplayerMenu;
	public YouDiedMenu youDiedMenu;

	private List<UIMenuPanelBase> allMenus => new List<UIMenuPanelBase> {inGameMenu, mainGameMenu, multiplayerMenu, youDiedMenu};

	private void Awake() {
		foreach (var initable in GetComponentsInChildren<Initable>(true))
			initable.Init();
		
		mainGameMenu.Open();
	}
	
	[RuntimeInitializeOnLoadMethod]
	private static void MakeSureInstanceIsActive() {
		instance.gameObject.SetActive(true);
	}
}
