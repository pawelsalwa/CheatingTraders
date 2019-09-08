using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class StartGameMenu : MonoBehaviour, Initable, IGameMenu {

	public MultiplayerMenu multiplayerMenu;

	public Button goMultiplayerBtn;
	public Button singlePlayerBtn;
	public Button quitBtn;

	public void Init() {
		goMultiplayerBtn.onClick.AddListener(() => {
			Close();
			multiplayerMenu.Open();
		});
		
		singlePlayerBtn.onClick.AddListener(() => {
			Close();
			GM.instance.StartSinglePlayerGame();
		});

		quitBtn.onClick.AddListener(Application.Quit);

	}

	public void Open() {
		gameObject.SetActive(true);
	}

	public void Close() {
		gameObject.SetActive(false);
	}
}