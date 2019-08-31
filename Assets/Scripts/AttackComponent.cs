using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> gets events from animation and orders weapon to deal damage </summary>
public class AttackComponent : MonoBehaviour {

	public Weapon weapon;


	public string attackingAnimatorKey = "attacking";
	public string shieldBlockingAnimatorKey = "shieldBlock";
	
	private Animator _animator;
	private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

	private bool animCanDealDamage = false;

	///<summary> plays attack animation allowing hit event from it. Should be called on update for anim to work</summary>
	public void ContinueToAttack() {
		animator.SetBool(attackingAnimatorKey, true);

		if (animCanDealDamage)
			weapon.DealDamageIfFoundTarget();
		else {
			weapon.StopDealingDamage();
		}
	}

	public void StopAttacking() {
		animator.SetBool(attackingAnimatorKey, false);
	}
	
	///<summary> plays attack animation allowing hit event from it. Should be called on update for anim to work</summary>
	public void ContinueToBlock() {
		animator.SetBool(shieldBlockingAnimatorKey, true);

//		if (animCanDealDamage)
//			weapon.DealDamageIfFoundTarget();
//		else {
//			weapon.StopDealingDamage();
//		}
	}

	public void StopBlocking() {
		animator.SetBool(shieldBlockingAnimatorKey, false);
	}

	///<summary>idzcie do animacji i tam szukajcie wywolania xD</summary>
	private void SwordHitTargetByAnim() {		
		animCanDealDamage = true;
	}

	private void SwordPassedThroughTargetByAnim() {		
		animCanDealDamage = false;
	}
}