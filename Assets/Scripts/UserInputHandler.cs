﻿using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

///<summary> Acts as layer of abstraction between user input and referenced components </summary>
public class UserInputHandler : NetworkBehaviour {
	public MovementComponent movement;
	public AttackComponent attack;
	public CharacterRotationComponent rotatation;

	private Transform cameraRot => GM.instance.mainCamera.gameObject.transform;

	private enum MoveDir { none, W, S, A, D, WA, WD, SA, SD }

	private MoveDir moveDir;

	private void Update() {
		if (GM.isGamePaused) return;
		HandleMisc();
		HandleMovement();
		HandleAttack();
	}

	private void LateUpdate() {
		HandleRotation();
	}

	private void HandleAttack() {
		if (Input.GetKey(KeyCode.Mouse1)) {
			attack.StopAttacking();
			attack.ContinueToBlock();
			return;
		} else {
			attack.StopBlocking();
		}

		if (Input.GetKey(KeyCode.Mouse0))
			attack.ContinueToAttack();
		else
			attack.StopAttacking();
	}

	private void HandleMovement() {
		SetMoveDir();

		switch (moveDir) {
			case MoveDir.none:
				movement.DontMove();
				break;
			case MoveDir.W:
				movement.MoveW();
				break;
			case MoveDir.S:
				movement.MoveS();
				break;
			case MoveDir.A:
				movement.MoveA();
				break;
			case MoveDir.D:
				movement.MoveD();
				break;
			case MoveDir.WD:
				movement.MoveWD();
				break;
			case MoveDir.WA:
				movement.MoveWA();
				break;
			case MoveDir.SD:
				movement.MoveSD();
				break;
			case MoveDir.SA:
				movement.MoveSA();
				break;
			default: break;
		}
	}

	private void SetMoveDir() {
		moveDir = MoveDir.none;

		if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) {
			moveDir = MoveDir.WD;
			return;
		}

		if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) {
			moveDir = MoveDir.WA;
			return;
		}

		if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)) {
			moveDir = MoveDir.SD;
			return;
		}

		if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) {
			moveDir = MoveDir.SA;
			return;
		}

		if (Input.GetKey(KeyCode.W)) {
			moveDir = MoveDir.W;
			return;
		}

		if (Input.GetKey(KeyCode.S)) {
			moveDir = MoveDir.S;
			return;
		}

		if (Input.GetKey(KeyCode.A)) {
			moveDir = MoveDir.A;
			return;
		}

		if (Input.GetKey(KeyCode.D)) {
			moveDir = MoveDir.D;
			return;
		}
	}

	private void HandleRotation() {
		rotatation.LookAt(new Vector3(cameraRot.forward.x, 0f, cameraRot.forward.z));
//        rotatation.AnimateRotation(Input.GetAxis("Horizontal"));
	}

	private void HandleMisc() {
		if (Input.GetKeyDown(KeyCode.Escape))
			UIManager.instance.inGameMenu.Open();
	}
}