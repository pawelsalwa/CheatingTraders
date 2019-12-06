using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

///<summary> Acts as layer of abstraction between user input and unit components that needs controlling </summary>
public class UserInputHandler : MonoBehaviour {

	public MovementComponent movement;
	public CombatComponent combat;
	public CharacterRotationComponent rotatation;
	public DodgingComponent dodgingComponent;

	private Transform cameraRot => GM.instance.mainCamera.gameObject.transform;

	public enum MoveDir { none, W, S, A, D, WA, WD, SA, SD }

	private MoveDir moveDir;

	private bool lShift => Input.GetKey(KeyCode.LeftShift);
	private bool mouse0 => Input.GetKey(KeyCode.Mouse0);
	private bool mouse1 => Input.GetKey(KeyCode.Mouse1);

	private void Update() {
		if (GM.isGamePaused) return;
		HandleMisc();
		HandleMovement();
		HandleAttack();
		HandleDodge();
	}

	private void LateUpdate() {
		HandleRotation();
	}

	private void HandleDodge() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			switch (moveDir) {
				case MoveDir.W: dodgingComponent.MoveW(); break;
				case MoveDir.S: dodgingComponent.MoveS(); break;
				case MoveDir.A: dodgingComponent.MoveA(); break;
				case MoveDir.D: dodgingComponent.MoveD(); break;
				case MoveDir.WD: dodgingComponent.MoveWD(); break;
				case MoveDir.WA: dodgingComponent.MoveWA(); break;
				case MoveDir.SD: dodgingComponent.MoveSD(); break;
				case MoveDir.SA: dodgingComponent.MoveSA(); break;
				default: dodgingComponent.DontMove(); break;
			}
		}
	}
	
	private void HandleAttack() {
		combat.SetBlockCommand(mouse1);
		if (mouse0 && !mouse1)
			combat.OrderToAttack();

	}

	private void HandleMovement() {
		SetMoveDir();
		movement.Move(moveDir, lShift);
	}

	private void SetMoveDir() {
		moveDir = MoveDir.none;

		if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) { moveDir = MoveDir.WD; return; }
		if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) { moveDir = MoveDir.WA; return; }
		if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)) { moveDir = MoveDir.SD; return; }
		if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) { moveDir = MoveDir.SA; return; }
		if (Input.GetKey(KeyCode.W)) { moveDir = MoveDir.W; return; }
		if (Input.GetKey(KeyCode.S)) { moveDir = MoveDir.S; return; }
		if (Input.GetKey(KeyCode.A)) { moveDir = MoveDir.A; return; }
		if (Input.GetKey(KeyCode.D)) { moveDir = MoveDir.D; return; }
	}

	private void HandleRotation() {
		rotatation.LookAt(new Vector3(cameraRot.forward.x, 0f, cameraRot.forward.z));
	}

	private void HandleMisc() {
		if (Input.GetKeyDown(KeyCode.Escape))
			UIManager.instance.inGameMenu.Open();
	}
}