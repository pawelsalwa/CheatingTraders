using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class MainGameMenu : UIMenuPanelBase {

	public MultiplayerMenu multiplayerMenu;

	public Button goMultiplayerBtn;
	public Button singlePlayerBtn;
	public Button quitBtn;

	protected override void Inited() {
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
}