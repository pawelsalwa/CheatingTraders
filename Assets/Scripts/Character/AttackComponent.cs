using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> gets events from animation and orders weapon to deal damage </summary>
public class AttackComponent : MonoBehaviour {

	public Weapon weapon;
	public Shield shield;

	public string attackingAnimatorKey = "attacking";
	public string shieldBlockingAnimatorKey = "shieldBlock";
	public string enemyShieldEncounteredAnimatorKey = "enemyShieldEncountered";
	public string onShieldImpactedAnimKey = "onShieldImpacted";
	
	private Animator _animator;
	private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

	private bool animCanDealDamage = false;
	private bool animCanImpactShield = false;

	private void Awake() {
		weapon.OnShieldEncountered += (shield) => { animator.SetTrigger(enemyShieldEncounteredAnimatorKey); };
		shield.OnShieldImpacted += TakeImpactFromBlock;
	}

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
	
	///<summary> plays block animation. Should be called on update for anim to work</summary>
	public void ContinueToBlock() {
		if (shield == null) return;
		animCanImpactShield = true;
		animator.SetBool(shieldBlockingAnimatorKey, true);
		shield.gameObject.SetActive(true);
	}

	public void StopBlocking() {
		if (shield == null) return;
		animCanImpactShield = false;
		animator.SetBool(shieldBlockingAnimatorKey, false);
		shield.gameObject.SetActive(false);
	}

	private void TakeImpactFromBlock() {
		Debug.Log("taking impact " + gameObject);
		animator.SetTrigger(onShieldImpactedAnimKey);
	}

	///<summary>idzcie do animacji i tam szukajcie wywolania xD</summary>
	private void SwordHitTargetByAnim() {		
		animCanDealDamage = true;
	}

	private void SwordPassedThroughTargetByAnim() {		
		animCanDealDamage = false;
	}
}