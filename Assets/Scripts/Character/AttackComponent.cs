using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> gets events from animation and orders weapon to deal damage (everything on update) </summary>
public class AttackComponent : MonoBehaviour {

	public event Action<bool> OnAttackCommand = isAttacking => { };
	public event Action<bool> OnBlockCommand = isBlocking => { };

	public Weapon weapon;
	public Shield shield;

	private bool animCanDealDamage = false;
	private bool animCanImpactShield = false;

//	private void Awake() {
//		weapon.OnShieldEncountered += (shield) => { animator.SetTrigger(enemyShieldEncounteredAnimatorKey); };
//		shield.OnShieldImpacted += TakeImpactFromBlock;
//	}

	///<summary> plays block animation. Should be called on update for anim to work</summary>
	public void SetBlockCommand(bool isBlocking) {
		if (shield == null) return;
		animCanImpactShield = isBlocking;
		shield.gameObject.SetActive(isBlocking);
		OnBlockCommand(isBlocking);
	}

	///<summary> plays attack animation allowing hit event from it. Should be called on update for anim to work</summary>
	public void SetAttackCommand(bool isAttacking) {
		if (!isAttacking) {
			OnAttackCommand(false);
			return;
		}
		
		if (animCanDealDamage)
			weapon.DealDamageIfFoundTarget();
		else 
			weapon.StopDealingDamage();
		
		OnAttackCommand(true);
	}
	
//	public void ContinueToBlock() {
//		if (shield == null) return;
//		animCanImpactShield = true;
//		animator.SetBool(shieldBlockingAnimatorKey, true);
//		shield.gameObject.SetActive(true);
//	}
//
//	public void StopBlocking() {
//		if (shield == null) return;
//		animCanImpactShield = false;
//		animator.SetBool(shieldBlockingAnimatorKey, false);
//		shield.gameObject.SetActive(false);
//	}

	private void TakeImpactFromBlock() {
		Debug.Log("taking impact " + gameObject);
//		animator.SetTrigger(onShieldImpactedAnimKey);
	}

	///<summary> Unity calls mono functions from animations by string :\ </summary>
	private void SwordHitTargetByAnim() {
		animCanDealDamage = true;
	}

	private void SwordPassedThroughTargetByAnim() {
		animCanDealDamage = false;
	}
}