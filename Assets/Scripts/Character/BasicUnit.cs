using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror;
using UnityEditor;
using UnityEngine;
using System;

public enum ControlledBy {
	Player,
	AI
}

[RequireComponent(typeof(Animator))]
public class BasicUnit : NetworkBehaviour {

	public event Action OnDeath = () => { };
	
	public HealthComponent hp;
	public AttackTarget attTarget;

	public string animDieKey;
	public int deadBodyTimeout = 2000;

	public Transform cameraOrbit;
	public Transform cameraFollow;

	private bool _isAlive = true;
	public bool isAlive => _isAlive;

	private Animator _animator;
	private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

	private UserInputHandler _userInputHandler;
	public UserInputHandler userInputHandler => _userInputHandler == null ? _userInputHandler = GetComponent<UserInputHandler>() : _userInputHandler;

	protected virtual void Start() {
		hp.OnHpDropBelowZero += Die;
		UIMenuPanelBase.OnAnyPanelOpened += () => { animator.speed = UIManager.isAnyMenuOpened ? 0f : 1f; GM.instance.cinemachineFreeLook.enabled = !UIManager.isAnyMenuOpened;};
		UIMenuPanelBase.OnAnyPanelClosed += () => { animator.speed = UIManager.isAnyMenuOpened ? 0f : 1f; GM.instance.cinemachineFreeLook.enabled = !UIManager.isAnyMenuOpened;};		
	}

	public virtual void InitAsPlayer() {
		userInputHandler.enabled = true;
		GetComponent<BotController>().enabled = false;
		GetComponent<CharacterController>().enabled = true;
		GM.instance.cinemachineFreeLook.m_Follow = cameraFollow;
		GM.instance.cinemachineFreeLook.m_LookAt = cameraOrbit;
		OnDeath += UIManager.instance.youDiedMenu.Open;
	}
	
	public void InitAsBot() {
		userInputHandler.enabled = false;
		GetComponent<BotController>().enabled = true;
		GetComponent<CharacterController>().enabled = true;
	}
	
	private void Die() {
		if (!_isAlive) // only shadows die twice 
			return;
		
		OnDeath();
		_isAlive = false;
		attTarget.isTargettable = false;
		animator.SetBool(animDieKey, true);
		foreach (var childCollider in GetComponentsInChildren<Collider>())
			childCollider.enabled = false;

		RemoveLoosersBody();
	}

	private async void RemoveLoosersBody() {
		await Task.Delay(deadBodyTimeout);
#if UNITY_EDITOR
		if (EditorApplication.isPlaying)
#endif 
		{
			Destroy(gameObject); // async code doesnt stop on play mode exit
		}
	}
}