using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitAnimationManager : MonoBehaviour {

	private Animator _animator;
	private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;
	
	[Header("Animation params")]
	[Range(0f, 1f)] public float moveXSmoothness = 0.5f;
	[Range(0f, 1f)] public float moveYSmoothness = 0.5f;
	
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
	public string attackingAnimatorKey = "attacking";
	public string shieldBlockingAnimatorKey = "shieldBlock";
	public string enemyShieldEncounteredAnimatorKey = "enemyShieldEncountered";
	public string onShieldImpactedAnimKey = "onShieldImpacted";

	public void SetMovementAnim(float xAnim, float yAnim, float speedAnimFactor) {
		animator.SetBool(isMovingKey, !(Mathf.Approximately(xAnim,0f) && Mathf.Approximately(yAnim,0f)));
		animator.SetLayerWeight(0, 1 - speedAnimFactor);
		animator.SetLayerWeight(1, speedAnimFactor);
		animator.SetFloat(verticalMovementKey, yAnim, moveYSmoothness, Time.deltaTime);
		animator.SetFloat(horizontalMovementKey, xAnim, moveXSmoothness, Time.deltaTime);
	}
	
	public void SetDodgeAnim(float yAnim, float xAnim) {
		if (Mathf.Approximately(yAnim, 0f) || Mathf.Approximately(yAnim, 0f)) // jesli rozne od zera
			animator.SetTrigger(dodgeTrigger);
		animator.SetFloat(dodgeFrontFactor, yAnim);
		animator.SetFloat(dodgeRightFactor, xAnim);
	}

	public void SetAttackAnim(bool attacking) {
		animator.SetBool(attackingAnimatorKey, attacking);
	}
	
	public void SetBlockAnim(bool blocking) {
		animator.SetBool(shieldBlockingAnimatorKey, blocking);
	}

	public void TakeDamage() {
		animator.SetTrigger(takeDamageTrigger);
	}

	public void Die() {
		animator.SetTrigger(animDieTrigger);
	}
}
