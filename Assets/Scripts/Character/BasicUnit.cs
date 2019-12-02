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
[RequireComponent(typeof(CombatComponent))]
[RequireComponent(typeof(UnitAnimationManager))]
public class BasicUnit : MonoBehaviour {

	public event Action OnDeath = () => { };

	public string animDieKey;
	public int deadBodyTimeout = 2;

	public Transform cameraOrbit;
	public Transform cameraFollow;

	public int playerLayer, botLayer;

	private bool _isAlive = true;
	public bool isAlive => _isAlive;
	
//	private bool _isStaggered = false;
//	public bool isStaggered => _isStaggered; //TODO: will be needed here?
	
	private HealthComponent _hp;
	private HealthComponent hp => _hp == null ? _hp = GetComponentInChildren<HealthComponent>(true) : _hp;
	
	private BodyTarget _bodyTarget;
	private BodyTarget bodyTarget => _bodyTarget == null ? _bodyTarget = GetComponentInChildren<BodyTarget>(true) : _bodyTarget;

	private MovementComponent _movementComponent;
	private MovementComponent movementComponent => _movementComponent == null ? _movementComponent = GetComponent<MovementComponent>() : _movementComponent;

	private UnitAnimationManager _animManager;
	private UnitAnimationManager animManager => _animManager == null ? _animManager = GetComponent<UnitAnimationManager>() : _animManager;
	
	private CombatComponent _combatComponent;
	private CombatComponent combatComponent => _combatComponent == null ? _combatComponent = GetComponent<CombatComponent>() : _combatComponent;
	
	private DodgingComponent _dodgingComponent;
	private DodgingComponent dodgingComponent => _dodgingComponent == null ? _dodgingComponent = GetComponent<DodgingComponent>() : _dodgingComponent;

	public UserInputHandler userInputHandler;
	public BotController botController;

	private void OnMenuChanged() {
//		animator.speed = UIManager.isAnyMenuOpened ? 0f : 1f;
		GM.instance.cinemachineFreeLook.enabled = !UIManager.isAnyMenuOpened;
	}

	public void InitAsPlayer() {
		hp.hp = 200;
		
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
		hp.hp = 111;
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

		_isAlive = false;
		animManager.Die();
		OnDeath();
		bodyTarget.isTargettable = false;
		foreach (var childCollider in GetComponentsInChildren<Collider>())
			childCollider.enabled = false;

//		RemoveLoosersBody();
		Invoke("RemoveLoosersBody", deadBodyTimeout);
	}

	private void RemoveLoosersBody() {
		Destroy(gameObject);
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
		
		bodyTarget.OnDamageTaken += HandleTakingDamage;
		movementComponent.OnMovementRequested += HandleMovement;
		dodgingComponent.OnDodgeRequested += animManager.SetDodgeAnim;
		combatComponent.OnAttackCommand += HandleAttack;
		combatComponent.OnBlockCommand += animManager.SetBlockAnim;

		animManager.staggeringEntered += movementComponent.DisableMovement;
		animManager.staggeringEnded += movementComponent.EnableMovement;
	}
	
	private void UnsubscribeEvents() {
		UIMenuPanelBase.OnAnyPanelOpened -= OnMenuChanged;
		UIMenuPanelBase.OnAnyPanelClosed -= OnMenuChanged;
		
		bodyTarget.OnDamageTaken -= HandleTakingDamage;
		movementComponent.OnMovementRequested -= HandleMovement;
		dodgingComponent.OnDodgeRequested -= animManager.SetDodgeAnim;
		combatComponent.OnAttackCommand -= HandleAttack;
		combatComponent.OnBlockCommand -= animManager.SetBlockAnim;
	}

	private void HandleMovement(float xAnim, float yAnim, float speedAnimFactor) {
		animManager.SetMovementAnim(xAnim, yAnim, speedAnimFactor);
	}

	private void HandleTakingDamage(int damage) {
		combatComponent.DisableDealingDamage();
		
		if (hp.TakeDamage(damage).isHpBelowZero) Die();
		else animManager.TakeDamageAnim();
	}

	private void HandleAttack(bool isAttacking) {
		animManager.SetAttackAnim(isAttacking);
	}
}