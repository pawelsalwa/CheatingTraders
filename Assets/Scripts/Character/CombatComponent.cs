﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> gets events from animation and orders weapon to deal damage (everything on update) </summary>
public class CombatComponent : MonoBehaviour {

	public event Action<bool> OnAttackCommand = isAttacking => { };
	public event Action<bool> OnBlockCommand = isBlocking => { };
	public event Action<Shield> OnEnemyShieldEncounter = shield => { };
	public event Action OnShieldImpact = () => { };

	public bool m_isAttacking { get; private set; } = false;
	public bool m_isBlocking { get; private set; } = false;

	[SerializeField] private Weapon weapon;
	[SerializeField] private Shield shield;

	[SerializeField] private  bool enableDebugs = false;

	private bool canDealDamageByAnim = false;
	private bool animCanImpactShield = false;


	///<summary> plays block animation. Should be called on update for anim to work</summary>
	public void SetBlockCommand(bool isBlocking) {
		if (shield == null) return;

		m_isBlocking = isBlocking;
		animCanImpactShield = isBlocking;
		shield.gameObject.SetActive(isBlocking);
		OnBlockCommand(isBlocking);
	}

	///<summary> plays attack animation allowing hit event from it. Should be called on update for anim to work</summary>
	public void SetAttackCommand(bool isAttacking) {
		m_isAttacking = isAttacking;
		
		if (isAttacking) {
			if (canDealDamageByAnim)
				weapon.DealDamageIfFoundTarget();
			else 
				weapon.StopDealingDamage();
		} else {
//			if (canDealDamageByAnim) //TODO: test it 
			DisableDealingDamage();
		}
		
		
		OnAttackCommand(isAttacking);
	}
	
	public void DisableDealingDamage() {
		if(enableDebugs)
			Debug.Log("<color=red>DisabledAttack</color> " + gameObject.name, gameObject);
		canDealDamageByAnim = false;
	}
	
	private void EnableDealingDamage() {
		if(enableDebugs)
			Debug.Log("<color=green>EnabledAttack</color> " + gameObject.name, gameObject);
		canDealDamageByAnim = true;
	}
	
	
	///<summary> Unity calls those mono functions from animations by string :\ </summary>
	private void SwordHitTargetByAnim() {
		EnableDealingDamage();
	}
	
	private void SwordPassedThroughTargetByAnim() {
		DisableDealingDamage();
	}
	
	private void Awake() {
		weapon.OnEnemyShieldEncounter += EnemyShieldEncounter;
		shield.OnShieldImpacted += TakeImpactFromBlock;
	}

	private void EnemyShieldEncounter(Shield shield) {
		DisableDealingDamage();
		OnEnemyShieldEncounter(shield);
	}

	private void TakeImpactFromBlock() {
		if (enableDebugs)
			Debug.Log("taking impact " + gameObject.name, gameObject);
		OnShieldImpact();
	}
}