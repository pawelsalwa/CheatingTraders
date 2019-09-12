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
		backToMainMenuBtn.onClick.AddListener(() => { 
			Close();
			GM.instance.EndGame();
			mainGameMenu.Open(); 
		});
	}

	public void Update() {
		if (isOpened && Input.GetKeyDown(KeyCode.Escape)) 
			ReturnToGame();
	}

	private void ReturnToGame() {
		Close(); 
	}
}