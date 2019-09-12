using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YouDiedMenu : UIMenuPanelBase {

	public MainGameMenu mainGameMenu;

	public Button goToMenuBtn;
	public Button quitBtn;

	protected override void Inited() {
		goToMenuBtn.onClick.AddListener(() => {
			Close();
			GM.instance.EndGame();
			mainGameMenu.Open();
		});

		quitBtn.onClick.AddListener(Application.Quit);
	}
}