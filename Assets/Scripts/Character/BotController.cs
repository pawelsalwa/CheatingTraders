using System;
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
			if (_combatState == value)
				return;
			//Debug.Log(_combatState.ToString());
			_combatState = value;
		}
	}

	private BasicUnit _thisUnit;
	private BasicUnit thisUnit => _thisUnit == null ? _thisUnit = GetComponent<BasicUnit>() : _thisUnit;

	private MovementComponent _movement;
	private MovementComponent movement => _movement == null ? _movement = GetComponent<MovementComponent>() : _movement;

	private AttackComponent _attack;
	private AttackComponent attack => _attack == null ? _attack = GetComponent<AttackComponent>() : _attack;

	private CharacterRotationComponent _rotatation;
	private CharacterRotationComponent rotatation => _rotatation == null ? _rotatation = GetComponent<CharacterRotationComponent>() : _rotatation;

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
	private bool isInCombatDist => distanceToTarget < combatModeQuitDistance && inCombatMode;

	private Transform player => GM.player?.transform;
	private Vector3 thisPos;
	private Vector3 targetPos;
	private Vector3 targetDir;

	private bool shiftPressed = false;

	private void Update() {
		if (!IsValid()) {
			attack.SetAttackCommand(false);
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
		return !GM.isGamePaused && thisUnit.isAlive;
	}

	private void SeekTarget() {
		if (player == null) return;

		thisPos = transform.position + Vector3.up;
		targetPos = player.position + Vector3.up;
		targetDir = targetPos - thisPos;

		Debug.DrawRay(thisPos, targetDir, Color.white, Time.deltaTime, true);
		int attackableBodyLayer = 9;

		if (!Physics.Raycast(thisPos, targetDir, out var hitInfo, Mathf.Infinity, Mask)) {
			currentTarget = null;
			return;
		}

		if (!hitInfo.transform.CompareTag("Player")) {
			currentTarget = null;
			return;
		}

		if ((distanceToTarget = hitInfo.distance) > enemyDetectionDistance) {
			currentTarget = null;
			return;
		}

		currentTarget = hitInfo.transform.GetComponentInParent<BasicUnit>(); //TODO: legitny system łapania targetu (moze nawet bez tagow)
		currentTarget = currentTarget.isAlive ? currentTarget : null;
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
		combatState = (CombatState) (Random.Range(0, 1000) % 2) + 2;
//		shiftPressed = Random.Range(0, 2) == 1;
	}

	private void ExecuteCombatAction() {
		switch (combatState) {
			case CombatState.MoveW:
				if (distanceToTarget >= attackStartDistance)
					movement.MoveW(shiftPressed);
				timeBetweenAiActionChange = 1f;
				break;
			case CombatState.MoveS:
				movement.MoveS(shiftPressed);
				timeBetweenAiActionChange = 1f;
				break;
			case CombatState.StrafeA:
				movement.MoveA(shiftPressed);
				timeBetweenAiActionChange = 1f;
				break;
			case CombatState.StrafeD:
				movement.MoveD(shiftPressed);
				timeBetweenAiActionChange = 1f;
				break;
		}
	}

	private void AttackIfInRange() {
		if (distanceToTarget < attackStartDistance) attack.SetAttackCommand(true);
		else attack.SetAttackCommand(false);
	}
}