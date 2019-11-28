using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Xml.Serialization;

[RequireComponent(typeof(MovementComponent))]
[RequireComponent(typeof(DodgingComponent))]
[RequireComponent(typeof(AttackComponent))]
[RequireComponent(typeof(UnitAnimationManager))]
public class BasicUnit : MonoBehaviour {

	public event Action OnDeath = () => { };

	public HealthComponent hp;
	public BodyTarget attTarget;

	public string animDieKey;
	public int deadBodyTimeout = 2000;

	public Transform cameraOrbit;
	public Transform cameraFollow;

	public int playerLayer, botLayer;

	private bool _isAlive = true;
	public bool isAlive => _isAlive;

	private MovementComponent _movementComponent;
	private MovementComponent movementComponent => _movementComponent == null ? _movementComponent = GetComponent<MovementComponent>() : _movementComponent;

	private UnitAnimationManager _animManager;
	private UnitAnimationManager animManager => _animManager == null ? _animManager = GetComponent<UnitAnimationManager>() : _animManager;
	
	private AttackComponent _attackComponent;
	private AttackComponent attackComponent => _attackComponent == null ? _attackComponent = GetComponent<AttackComponent>() : _attackComponent;
	
	private DodgingComponent _dodgingComponent;
	private DodgingComponent dodgingComponent => _dodgingComponent == null ? _dodgingComponent = GetComponent<DodgingComponent>() : _dodgingComponent;

	public UserInputHandler userInputHandler;
	public BotController botController;


	private void OnMenuChanged() {
//		animator.speed = UIManager.isAnyMenuOpened ? 0f : 1f;
		GM.instance.cinemachineFreeLook.enabled = !UIManager.isAnyMenuOpened;
	}

	public void InitAsPlayer() {
		userInputHandler.enabled = true;
		GetComponent<BotController>().enabled = false;
		GetComponent<CharacterController>().enabled = true;
		GM.instance.cinemachineFreeLook.m_Follow = cameraFollow;
		GM.instance.cinemachineFreeLook.m_LookAt = cameraOrbit;
		OnDeath += UIManager.instance.youDiedMenu.Open;
		SetLayer(playerLayer);
		gameObject.name = "-- PlayerUnit --";
	}

	public void InitAsBot() {
		userInputHandler.enabled = false;
		GetComponent<BotController>().enabled = true;
		GetComponent<CharacterController>().enabled = true;
		SetLayer(botLayer);
		gameObject.name = "-- BotUnit --";
	}

	private void SetLayer(int layer) {
		foreach (var child in GetComponentsInChildren<Transform>(true)) child.gameObject.layer = layer;
	}

	private void Die() {
		if (!_isAlive) // only shadows die twice 
			return;

		OnDeath();
		_isAlive = false;
		attTarget.isTargettable = false;
		animManager.Die();
		foreach (var childCollider in GetComponentsInChildren<Collider>())
			childCollider.enabled = false;

		RemoveLoosersBody();
	}

	private void RemoveLoosersBody() {
//		await Task.Delay(deadBodyTimeout);
//#if UNITY_EDITOR
//		if (EditorApplication.isPlaying)
//#endif 
//		{
		Destroy(gameObject, deadBodyTimeout); // async code doesnt stop on play mode exit
//		}
	}

	private void OnEnable() {
		SubscribeToEvents();
	}

	private void OnDisable() {
		UnsubscribeEvents();
	}

	private void SubscribeToEvents() {
		UIMenuPanelBase.OnAnyPanelOpened += OnMenuChanged;
		UIMenuPanelBase.OnAnyPanelClosed += OnMenuChanged;
		hp.OnHpDropBelowZero += Die;
		
		attTarget.OnDamageTaken += _ => { animManager.TakeDamage(); };
		movementComponent.OnMovementRequested += animManager.SetMovementAnim;
		dodgingComponent.OnDodgeRequested += animManager.SetDodgeAnim;
		attackComponent.OnAttackCommand += animManager.SetAttackAnim;
		attackComponent.OnBlockCommand += animManager.SetBlockAnim;
	}
	
	private void UnsubscribeEvents() {
		UIMenuPanelBase.OnAnyPanelOpened -= OnMenuChanged;
		UIMenuPanelBase.OnAnyPanelClosed -= OnMenuChanged;
		hp.OnHpDropBelowZero -= Die;
		movementComponent.OnMovementRequested -= animManager.SetMovementAnim;
		dodgingComponent.OnDodgeRequested -= animManager.SetDodgeAnim;
	}

}