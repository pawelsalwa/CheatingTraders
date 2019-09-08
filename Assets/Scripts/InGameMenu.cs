using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour, Initable {

	public event Action OnOpened = () => { };
	public event Action OnClosed = () => { };

	public Button ResumeGameBtn;
	public Button backToMainMenuBtn;
	
	public MainGameMenu mainGameMenu;

	private bool _isOpened;

	public bool isOpened {
		get => _isOpened;
		private set {
			_isOpened = value;
			GM.EnableCamera(!value);
		}
	}

	public void Init() {
		ResumeGameBtn.onClick.AddListener(Close);
		backToMainMenuBtn.onClick.AddListener(() => {
			Close();
			mainGameMenu.Open();
		});
		Close();
	}

	public void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (isOpened) {
				Close();
			} else {
				Open();
			}
		}
	}

	private void Open() {
		isOpened = true;
		gameObject.SetActive(true);
//		Cursor.lockState = CursorLockMode.Confined;
		OnOpened();
	}

	private void Close() {
		isOpened = false;
		gameObject.SetActive(false);
//		Cursor.lockState = CursorLockMode.Locked;
	}
}