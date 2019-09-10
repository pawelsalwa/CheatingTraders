using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : UIMenuPanelBase {

	public Button ResumeGameBtn;
	public Button backToMainMenuBtn;
	
	public MainGameMenu mainGameMenu;

	protected override void Inited() {
		ResumeGameBtn.onClick.AddListener(ReturnToGame);
		backToMainMenuBtn.onClick.AddListener(() => { Close(); mainGameMenu.Open(); });
	}

	public void Update() {
		if (isOpened && Input.GetKeyDown(KeyCode.Escape)) 
			ReturnToGame();
	}

	protected override void Opened() {
		GM.isGamePaused = true;
	}

	private void ReturnToGame() {
		Close(); 
		GM.isGamePaused = false;
	}
}