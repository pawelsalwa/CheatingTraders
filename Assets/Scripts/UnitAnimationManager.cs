using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitAnimationManager : MonoBehaviour {

	public event Action staggeringEntered = () => { };
	public event Action staggeringEnded = () => { };
	public event Action OnShieldImpactEnded = () => { };

	private Animator _animator;
	private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;
	
	[Header("Animation params")]
	[Range(0f, 1f)] public float moveXSmoothness = 0.5f;
	[Range(0f, 1f)] public float moveYSmoothness = 0.5f;
	[Range(0f, 3f)] public float staggerDurationSecs = 1f;
	[Range(0f, 3f)] public float currentStaggerDurForDebug = 0f;
	
	[Header("Animator keys")]
	public string takeDamageTrigger = "takeDamageTrigger";
	public string animDieTrigger = "dieTrigger";
	[Space(10)]
	public string verticalMovementKey = "moveA";
	public string horizontalMovementKey = "moveW";
	public string isMovingKey = "moving";
	[Space(10)]
	public string dodgeTrigger = "dodgeTrigger";
	public string dodgeFrontFactor = "dodgeFrontFactor";
	public string dodgeRightFactor = "dodgeRightFactor";
	[Space(10)]
//	public string attackingAnimatorKey = "attacking";
	public string attackTriggerKey = "attackTrigger";
	public string shieldBlockingAnimatorKey = "shieldBlock";
	public string enemyShieldEncounteredAnimatorKey = "enemyShieldEncounteredTrigger";
	public string shieldTakingImpactAnimKey = "shieldTakeImpactTrigger";
	public string isStaggeringAnimatorKey = "isStaggering";


	public void SetMovementAnim(float xAnim, float yAnim, float speedAnimFactor) {
		animator.SetBool(isMovingKey, !(Mathf.Approximately(xAnim,0f) && Mathf.Approximately(yAnim,0f)));
		animator.SetLayerWeight(0, 1 - speedAnimFactor);
		animator.SetLayerWeight(1, speedAnimFactor);
		animator.SetFloat(verticalMovementKey, yAnim, moveYSmoothness, Time.deltaTime);
		animator.SetFloat(horizontalMovementKey, xAnim, moveXSmoothness, Time.deltaTime);
	}
	
	public void SetDodgeAnim(float yAnim, float xAnim) {
		if (!Mathf.Approximately(yAnim, 0f) || !Mathf.Approximately(xAnim, 0f)) // jesli rozne od zera
			animator.SetTrigger(dodgeTrigger);
		animator.SetFloat(dodgeFrontFactor, yAnim);
		animator.SetFloat(dodgeRightFactor, xAnim);
	}

	public void SetAttackAnim() {
		animator.SetTrigger(attackTriggerKey);
//		animator.SetBool(attackingAnimatorKey, attacking);
	}
	
	public void SetBlockAnim(bool blocking) {
		animator.SetBool(shieldBlockingAnimatorKey, blocking);
	}

	public void TakeDamageAnim() {
		animator.SetTrigger(takeDamageTrigger);
		animator.SetBool(isStaggeringAnimatorKey, true);
		staggeringEntered();
		currentStaggerDurForDebug = 0;
		CancelInvoke("ResetStagger");
		Invoke("ResetStagger", staggerDurationSecs);
	}
	
	public void TakeShieldImpact() {
		animator.SetTrigger(shieldTakingImpactAnimKey);
		Invoke("ShieldImpactEnded", 1f);
	}

	public void Die() {
		animator.SetTrigger(animDieTrigger);
	}
	
	public void SetEnemyShieldEncounteredAnim() {
		animator.SetTrigger(enemyShieldEncounteredAnimatorKey);
	}

	private void ShieldImpactEnded() {
		CancelInvoke("ShieldImpactEnded");
		OnShieldImpactEnded();
	}

	private void ResetStagger() {
		animator.SetBool(isStaggeringAnimatorKey, false);
		staggeringEnded();
	}

	private void Update() {
		currentStaggerDurForDebug += Time.deltaTime;
		currentStaggerDurForDebug = Mathf.Clamp(currentStaggerDurForDebug, 0f, 3f);
	}

	private void OnEnable() {
		UIMenuPanelBase.OnAnyPanelChanged += UpdateAnimState;
	}

	private void OnDisable() {
		UIMenuPanelBase.OnAnyPanelChanged -= UpdateAnimState;
	}

	private void UpdateAnimState(){
		animator.enabled = !UIManager.isAnyMenuOpened;
	}
}
