using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuPanelBase : MonoBehaviour, Initable {
	
	public static event Action OnAnyPanelOpened = () => { };
	public static event Action OnAnyPanelClosed = () => { };
	
	public event Action OnOpened = () => { };
	public event Action OnClosed = () => { };

	public bool isOpened { get; private set; }

	public void Init() {
		Close();
		Inited();
	}

	public void Open() {
		isOpened = true;
		gameObject.SetActive(true);
//		Cursor.lockState = CursorLockMode.Confined;
		Opened();
		OnOpened();
		OnAnyPanelOpened();
	}
	
	public void Close() {
		isOpened = false;
		gameObject.SetActive(false);
//		Cursor.lockState = CursorLockMode.Locked;
		Closed();
		OnClosed();
		OnAnyPanelClosed();
	}

	protected virtual void Inited() { }

	protected virtual void Opened() { }
	
	protected virtual void Closed() { }
}
