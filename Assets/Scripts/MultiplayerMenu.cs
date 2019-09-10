﻿using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerMenu : UIMenuPanelBase {

	public MainGameMenu mainGameMenu;
	public NetworkManager manager;

	public Button hostLanBtn;
	public Button joinLanBtn;
	public Button startServerBtn;
	public Button backBtn;

	protected override void Inited() {
		hostLanBtn.onClick.AddListener(() => {
			Close();
			manager.StartHost();
		});
		joinLanBtn.onClick.AddListener(() => {
			Close();
			manager.StartClient();
		});
		startServerBtn.onClick.AddListener(() => {
			Close();
			manager.StartServer();
		});
		backBtn.onClick.AddListener(() => {
			mainGameMenu.Open();
			Close();
		});
	}
}