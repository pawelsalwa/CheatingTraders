using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : MonoBehaviour {

	public Weapon weapon;
	private Animator _animator;

	private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

	private bool animCanDealDamage = false;

	///<summary>plays attack animation allowing hit event from it</summary>
	public void ContinueToAttack() {
		animator.SetBool("attacking", true);

		if (animCanDealDamage)
			weapon.DealDamageIfFoundTarget();
	}

	public void StopAttacking() {
		animator.SetBool("attacking", false);
	}

	///<summary>idzcie do animacji i tam szukajcie xD</summary>
	private void SwordHitTargetByAnim() {
		// Debug.Log("SwordHitTargetByAnim");
		animCanDealDamage = true;
	}

	private void SwordPassedThroughTargetByAnim() {
		// Debug.Log("SwordPassedThroughTargetByAnim");
		animCanDealDamage = false;
	}
}