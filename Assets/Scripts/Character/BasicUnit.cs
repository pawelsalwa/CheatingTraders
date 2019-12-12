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
[RequireComponent(typeof(BotController))]
[RequireComponent(typeof(UserInputHandler))]
[RequireComponent(typeof(StaminaComponent))]
public class BasicUnit : MonoBehaviour {

	public event Action OnDeath = () => { };
	public event Action<float> OnStaminaChanged = stamina => { };
	public event Action OnNotEnoughStaminaForAction = () => { };

	public bool isAttacking => combatComponent.m_isAttacking;
	public bool isBlocking => combatComponent.m_isBlocking;

	[SerializeField]
	private int deadBodyTimeout = 2;

	public Weapon weapon;

	public Transform cameraOrbit;
	public Transform cameraFollow;

	[SerializeField]
	private int playerLayer, botLayer;

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
	
	private StaminaComponent _staminaComponent;
	private StaminaComponent staminaComponent => _staminaComponent == null ? _staminaComponent = GetComponent<StaminaComponent>() : _staminaComponent;
	
	private UserInputHandler _userInputHandler;
	private UserInputHandler userInputHandler => _userInputHandler == null ? _userInputHandler = GetComponent<UserInputHandler>() : _userInputHandler;
	
	private BotController _botController;
	private BotController botController => _botController == null ? _botController = GetComponent<BotController>() : _botController;

	public void InitAsPlayer() {
		hp.hp = 2000;		
		userInputHandler.enabled = true;
		botController.enabled = false;
		GM.instance.cinemachineFreeLook.m_Follow = cameraFollow;
		GM.instance.cinemachineFreeLook.m_LookAt = cameraOrbit;
		OnDeath += UIManager.instance.youDiedMenu.Open;
		SetLayer(playerLayer);
		SetTag("Player");
		gameObject.name = "-- PlayerUnit --";
		staminaComponent.OnStaminaChanged += stamina => OnStaminaChanged(stamina);
		staminaComponent.OnNotEnoughStaminaForAction += () => OnNotEnoughStaminaForAction();
//		weapon.enableDebugs = true;
	}

	public void InitAsBot() {
		hp.hp = 5555;
		userInputHandler.enabled = false;
		botController.enabled = true;
		SetLayer(botLayer);
		SetTag("Enemy");
		gameObject.name = "-- BotUnit --";
	}

	private void SetLayer(int layer) {
		foreach (var child in GetComponentsInChildren<Transform>(true)) child.gameObject.layer = layer;
	}

	private void SetTag(string tag) {
		foreach (var child in GetComponentsInChildren<Transform>(true)) child.gameObject.tag = tag;
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
		dodgingComponent.OnDodgeRequested += HandleDodge;
		
		combatComponent.OnAttackCommand += HandleAttack;
		combatComponent.OnBlockCommand += animManager.SetBlockAnim;
		combatComponent.OnEnemyShieldEncounter += HandleEncounteringEnemyShield;
		combatComponent.OnShieldImpact += TakeShieldImpact;

		animManager.staggeringEntered += movementComponent.DisableMovement;
		animManager.staggeringEnded += movementComponent.EnableMovement;
		animManager.OnShieldImpactEnded += () => movementComponent.SetMoveFactor(1f);
	}
	
	private void UnsubscribeEvents() {
		UIMenuPanelBase.OnAnyPanelOpened -= OnMenuChanged;
		UIMenuPanelBase.OnAnyPanelClosed -= OnMenuChanged;
		
		bodyTarget.OnDamageTaken -= HandleTakingDamage;
		movementComponent.OnMovementRequested -= HandleMovement;
		dodgingComponent.OnDodgeRequested -= HandleDodge;
		combatComponent.OnAttackCommand -= HandleAttack;
		combatComponent.OnBlockCommand -= animManager.SetBlockAnim;
	}

	private void HandleMovement(float xAnim, float yAnim, float speedAnimFactor) {
		animManager.SetMovementAnim(xAnim, yAnim, speedAnimFactor);
	}

	private void HandleTakingDamage(int damage) {
		combatComponent.DisableDealingDamage();
		hp.TakeDamage(damage);
		
		if (hp.isHpBelowZero) Die();
		else animManager.TakeDamageAnim();
	}

	private void HandleAttack() {
		animManager.SetAttackAnim();
	}

	private void HandleEncounteringEnemyShield(Shield enemyShield) {
		animManager.SetEnemyShieldEncounteredAnim();
	}
	
	private void TakeShieldImpact() {
		movementComponent.SetMoveFactor(0.5f);
		animManager.TakeShieldImpact();
	}

	private void HandleDodge(float yAnim, float xAnim) {
		if (!staminaComponent.AllowDodge()) return;

		animManager.SetDodgeAnim(yAnim, xAnim);
	}

	private void OnMenuChanged() {
		GM.instance.cinemachineFreeLook.enabled = !UIManager.isAnyMenuOpened;
	}
}