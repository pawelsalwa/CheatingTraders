using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour, Initable {

	public event Action OnOpened = () => { };
	public event Action OnClosed = () => { };

	public Button closeBtn;
	public GameObject container;

	private bool _isOpened;

	public bool isOpened {
		get => _isOpened;
		private set {
			_isOpened = value;
			GM.EnableCamera(!value);
		}
	}

	public void Init() {
		closeBtn.onClick.AddListener(Close);
//		Cursor.lockState = CursorLockMode.Locked;
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
		container.SetActive(true);
//		Cursor.lockState = CursorLockMode.Confined;
		OnOpened();
	}

	private void Close() {
		isOpened = false;
		container.SetActive(false);
//		Cursor.lockState = CursorLockMode.Locked;
		OnClosed();
	}
}