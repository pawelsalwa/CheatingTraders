using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BotController : MonoBehaviour {
	
	private enum CombatState {
		StrafeA,
		StrafeD,
		MoveW,
		MoveS
	}

	private CombatState combatState;

	public MovementComponent movement;
	public AttackComponent attack;
	public CharacterRotationComponent rotatation;

	public BasicUnit currentTarget;
	public float distanceToTarget;
	public float strafeDistance = 5f;
	private float timeBetweenActions = 2f;
	private float cTimeBetweenActions = 0f;
	private int action;

	private Transform player => GM.player.transform;

	private Vector3 thisPos;
	private Vector3 targetPos;
	private Vector3 targetDir;

	private bool isInCombatDist => Mathf.Abs(distanceToTarget - strafeDistance) < 0.4f;

	private void Update() {
		SeekTarget();
		
		if (currentTarget == null)
			return;

		LookAtTarget();

		if (isInCombatDist) {
			SetRandomCombatAction();
		}
		
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
			distanceToTarget = hitInfo.distance;
		}
	}


	private void Chase() {
		if (Mathf.Approximately(cTimeBetweenActions, timeBetweenActions)) {
			action = Random.Range(0, 1000) % 2;
			cTimeBetweenActions = 0f;
		}

		(action == 0 && isInCombatDist ? (Action) movement.MoveW : movement.MoveS)();
	}

	private void Strafe() {
		if (Mathf.Approximately(cTimeBetweenActions, timeBetweenActions)) {
			action = Random.Range(0, 1000) % 2;
			cTimeBetweenActions = 0f;
		}

		(action == 0 ? (Action) movement.MoveA : movement.MoveD)();
	}

	private void AttackIfInRange() {
		if (distanceToTarget < 1.3f) attack.ContinueToAttack();
	}

	private void LookAtTarget() {
		rotatation.LookAt(player.position - transform.position);
	}

	private void SetRandomCombatAction() {
		cTimeBetweenActions += Time.deltaTime;
		cTimeBetweenActions = Mathf.Clamp(cTimeBetweenActions, 0f, timeBetweenActions);

		if (!Mathf.Approximately(cTimeBetweenActions, timeBetweenActions)) 
			return;
		
		action = Random.Range(0, 1000) % 2;
		cTimeBetweenActions = 0f;
	}

	public enum BotState {
		Idle,
		Chase
	}

	
}