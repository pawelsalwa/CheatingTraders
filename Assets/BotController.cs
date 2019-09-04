using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BotController : MonoBehaviour {
	
	private enum CombatState {StrafeA,StrafeD,MoveW,MoveS}
	private CombatState _combatState;
	private CombatState combatState {
		get => _combatState;
		set {
//			if (_combatState == value) 
//				return;
			Debug.Log(_combatState.ToString());

			if (value == CombatState.MoveS)
				_combatState = CombatState.MoveW;
			else
				_combatState = value;
		}
	}

	public MovementComponent movement;
	public AttackComponent attack;
	public CharacterRotationComponent rotatation;

	public BasicUnit currentTarget;
	public float distanceToTarget;
	public float strafeDistance = 5f;
	private float timeBetweenActions = 1f;
	private float cTimeBetweenActions = 0f;

	private Transform player => GM.player.transform;

	private Vector3 thisPos;
	private Vector3 targetPos;
	private Vector3 targetDir;

	private bool isInCombatDist => distanceToTarget < strafeDistance;

	private void Update() {
		SeekTarget();
		
		if (currentTarget == null)
			return;

		LookAtTarget();

		if (isInCombatDist)
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
		Physics.Raycast(thisPos, targetDir, out var hitInfo);
		if (hitInfo.transform.CompareTag("Player")) {
			currentTarget = hitInfo.transform.GetComponentInParent<BasicUnit>(); //TODO: legitny system łapania targetu (moze nawet bez tagow)
			currentTarget = currentTarget.isAlive ? currentTarget : null;
			distanceToTarget = hitInfo.distance;
		}
	}


	private void Chase() {
		movement.MoveW();
	}

	private void Strafe() {
		movement.MoveD();
	}

	private void AttackIfInRange() {
		if (distanceToTarget < 1.8f) attack.ContinueToAttack();
		else attack.StopAttacking();
	}

	private void LookAtTarget() {
		rotatation.LookAt(player.position - transform.position);
	}

	private void SetRandomCombatAction() {
		cTimeBetweenActions += Time.deltaTime;
		cTimeBetweenActions = Mathf.Clamp(cTimeBetweenActions, 0f, timeBetweenActions);

		if (!Mathf.Approximately(cTimeBetweenActions, timeBetweenActions)) 
			return;
		
		cTimeBetweenActions = 0f;
		combatState = (CombatState) (Random.Range(0, 1000) % 4);
	}

	private void ExecuteCombatAction() {
		switch (combatState) {
			case CombatState.MoveW: movement.MoveW(); break;
			case CombatState.MoveS: movement.MoveS(); break;
			case CombatState.StrafeA: movement.MoveA(); break;
			case CombatState.StrafeD: movement.MoveD(); break;
		}
	}
}