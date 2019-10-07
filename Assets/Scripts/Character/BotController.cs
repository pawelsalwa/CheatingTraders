﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BasicUnit))]
public class BotController : MonoBehaviour {
	private enum CombatState {
		StrafeA,
		StrafeD,
		MoveW,
		MoveS
	}

	private CombatState _combatState;
	private CombatState combatState {
		get => _combatState;
		set {
			if(_combatState == value)
				return;
			//Debug.Log(_combatState.ToString());
			_combatState = value;
		}
	}

	public MovementComponent movement;
	public AttackComponent attack;
	
	public CharacterRotationComponent rotatation;
	public BasicUnit currentTarget;
	
	[Header("AI Config")]
	public float combatModeStartDistance = 5f;
	public float combatModeQuitDistance = 8f;
	
	public float timeBetweenAiActionChange = 1f;
	public float enemyDetectionDistance = 14f;
	public LayerMask Mask;
	
	public float attackStartDistance = 2f;
	public float attackQuitDistance = 2.2f;
	
	private float currentTimeBetweenActions = 0f;
	private float distanceToTarget;
	
	private bool inCombatMode = false;
	private bool isInCombatDist =>  distanceToTarget < combatModeQuitDistance && inCombatMode;
	
	private Transform player => GM.player?.transform;
	private Vector3 thisPos;
	private Vector3 targetPos;
	private Vector3 targetDir;
	
	private BasicUnit _thisUnit;
	private BasicUnit thisUnit => _thisUnit == null ? _thisUnit = GetComponent<BasicUnit>() : _thisUnit;


	private void Update() {
		if (!IsValid()) {
			attack.StopAttacking();
			return;
		}
			
		SeekTarget();

		if (currentTarget == null) {
			movement.DontMove();	
			return;
		}

		LookAtTarget();
		UpdateIfInCombatMode();

		if (inCombatMode)
			SetRandomCombatAction();
		else
			Chase();

		ExecuteCombatAction();
		AttackIfInRange();
	}

	private bool IsValid() {
		if (GM.isGamePaused) return false;
		if (!thisUnit.isAlive) return false;
		return true;
	}

	private void SeekTarget() {
		if (player == null) return;
		
		thisPos = transform.position + Vector3.up;
		targetPos = player.position + Vector3.up;
		targetDir = targetPos - thisPos;

		Debug.DrawRay(thisPos, targetDir, Color.white, Time.deltaTime, true);
		int attackableBodyLayer = 9; 

		if (!Physics.Raycast(thisPos, targetDir, out var hitInfo, Mask)) return;
		if (!hitInfo.transform.CompareTag("Player")) {currentTarget = null; return;}
		if ((distanceToTarget = hitInfo.distance) > enemyDetectionDistance) {currentTarget = null; return;}
		
		currentTarget = hitInfo.transform.GetComponentInParent<BasicUnit>(); //TODO: legitny system łapania targetu (moze nawet bez tagow)
//		currentTarget = currentTarget.isAlive ? currentTarget : null;
	}


	private void Chase() {
		combatState = CombatState.MoveW;
	}

	private void Strafe() {
		movement.MoveD();
	}

	private void LookAtTarget() {
		if (!currentTarget.isAlive) return;
		rotatation.LookAt(player.position - transform.position);
	}

	private void UpdateIfInCombatMode() { //simple hysteresis
		if (distanceToTarget < combatModeStartDistance) inCombatMode = true;
		if (distanceToTarget > combatModeQuitDistance) inCombatMode = false;
	}

	private void SetRandomCombatAction() {
		currentTimeBetweenActions += Time.deltaTime;
		currentTimeBetweenActions = Mathf.Clamp(currentTimeBetweenActions, 0f, timeBetweenAiActionChange);

		if (!Mathf.Approximately(currentTimeBetweenActions, timeBetweenAiActionChange))
			return;

		currentTimeBetweenActions = 0f;
		combatState = (CombatState) (Random.Range(0, 1000) % 4);
	}

	private void ExecuteCombatAction() {
		switch (combatState) {
			case CombatState.MoveW:
				if (distanceToTarget >= attackStartDistance)
					movement.MoveW();
				timeBetweenAiActionChange = 1f;
				break;
			case CombatState.MoveS:
				movement.MoveS();
				timeBetweenAiActionChange = 1f;
				break;
			case CombatState.StrafeA:
				movement.MoveA();
				timeBetweenAiActionChange = 1f;
				break;
			case CombatState.StrafeD:
				movement.MoveD();
				timeBetweenAiActionChange = 1f;
				break;
		}
	}

	private void AttackIfInRange() {
		if (distanceToTarget < attackStartDistance) attack.ContinueToAttack();
		else attack.StopAttacking();
	}
}