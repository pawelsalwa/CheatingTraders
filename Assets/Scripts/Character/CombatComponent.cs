using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> gets events from animation and orders weapon to deal damage (everything on update) </summary>
public class CombatComponent : MonoBehaviour {

	public event Action OnAttackCommand = () => { };
	public event Action<bool> OnBlockCommand = isBlocking => { };
	public event Action<Shield> OnEnemyShieldEncounter = shield => { };
	public event Action OnShieldImpact = () => { };

	public bool m_isAttacking { get; private set;}
	public bool m_isBlocking { get; private set; } = false;

	[SerializeField] private Weapon weapon;
	[SerializeField] private Shield shield;

	[SerializeField] private  bool enableDebugs = false;
	[SerializeField, Range(0.1f, 3f)] private float cooldownBetweenAttacks = 1.3f;

	private bool combatEnabled = true;
//	private bool animCanImpactShield = false;

	///<summary> plays block animation. Should be called on update for anim to work</summary>
	public void SetBlockCommand(bool isBlocking) {
		if (!combatEnabled) return;

		m_isBlocking = isBlocking;
//		animCanImpactShield = isBlocking;
		shield.gameObject.SetActive(isBlocking);
		OnBlockCommand(isBlocking);

		if (isBlocking) {
			weapon.EndDealingDamage();
		}
	}

	public void OrderToAttack() {
		if (m_isAttacking) return;
		m_isAttacking = true;
		OnAttackCommand();
		Invoke(nameof(CooldownAttack), cooldownBetweenAttacks);
	}
	
	public void InterruptCombat() { // BUGFIX - jak wejdzie miecz w cialo a potem w tarcze ktora sie zenabluje mimo taking damage
		weapon.EndDealingDamage();
		SetBlockCommand(false);
		combatEnabled = false;
		Invoke(nameof(EnableCombatAgain), 1f);
	}
	
	///<summary> Unity calls those mono functions from animations by string :\ </summary>
	private void SwordHitTargetByAnim() {
		weapon.StartDealingDamage();
	}
	
	private void SwordPassedThroughTargetByAnim() {
		weapon.EndDealingDamage();
	}

	private void EnableCombatAgain() {
		combatEnabled = true;
	}
	
	private void CooldownAttack() {
		if (enableDebugs) 
			Debug.Log("Attack Cooldowned");
		
		m_isAttacking = false;
	}
	
	private void Awake() {
		weapon.OnEnemyShieldEncounter += EnemyShieldEncounter;
		shield.OnShieldImpacted += TakeImpactFromBlock;
	}

	private void EnemyShieldEncounter(Shield shield) {
		weapon.EndDealingDamage();
		OnEnemyShieldEncounter(shield);
	}

	private void TakeImpactFromBlock() {
		if (enableDebugs)
			Debug.Log("taking impact " + gameObject.name, gameObject);
		OnShieldImpact();
	}
}