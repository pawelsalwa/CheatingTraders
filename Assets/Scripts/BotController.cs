using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//[RequireComponent(typeof()),RequireComponent(typeof()),RequireComponent(typeof())]
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
			Debug.Log(_combatState.ToString());
			_combatState = value;
		}
	}

	public MovementComponent movement;
	public AttackComponent attack;
	
	public CharacterRotationComponent rotatation;
	public BasicUnit currentTarget;
	
	[Header("AI Config")]
	public float combatStartDistance = 5f;
	public float combatQuitDistance = 8f;
	
	public float timeBetweenAiActionChange = 1f;
	public float enemyDetectionDistance = 14f;
	
	private float attackRange = 1.3f;
	
	private float currentTimeBetweenActions = 0f;
	private float distanceToTarget;
	
	private bool inCombatMode = false;
	private bool isInCombatDist =>  distanceToTarget < combatQuitDistance && inCombatMode;
	
	private Transform player => GM.player.transform;
	private Vector3 thisPos;
	private Vector3 targetPos;
	private Vector3 targetDir;


	private void Update() {
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

	private void SeekTarget() {
		thisPos = transform.position + Vector3.up;
		targetPos = player.position + Vector3.up;
		targetDir = targetPos - thisPos;

		Debug.DrawRay(thisPos, targetDir, Color.white, Time.deltaTime, true);

		if (!Physics.Raycast(thisPos, targetDir, out var hitInfo)) return;
		if (!hitInfo.transform.CompareTag("Player")) {currentTarget = null; return;}
		if ((distanceToTarget = hitInfo.distance) > enemyDetectionDistance) {currentTarget = null; return;}
		
		currentTarget = hitInfo.transform.GetComponentInParent<BasicUnit>(); //TODO: legitny system łapania targetu (moze nawet bez tagow)
		currentTarget = currentTarget.isAlive ? currentTarget : null;
	}


	private void Chase() {
//		movement.MoveW();
		combatState = CombatState.MoveW;
	}

	private void Strafe() {
		movement.MoveD();
	}

	private void AttackIfInRange() {
		if (distanceToTarget < attackRange) attack.ContinueToAttack();
		else attack.StopAttacking();
	}

	private void LookAtTarget() {
		rotatation.LookAt(player.position - transform.position);
	}

	private void UpdateIfInCombatMode() { //simple hysteresis
		if (distanceToTarget < combatStartDistance) inCombatMode = true;
		if (distanceToTarget > combatQuitDistance) inCombatMode = false;
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
				if (distanceToTarget >= attackRange)
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
}