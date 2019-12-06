using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary> uses CharacterController to trigger movement </summary>
[RequireComponent(typeof(CharacterController))]
public class MovementComponent : MonoBehaviour {

	public event Action<float, float, float> OnMovementRequested = (horizontal, vertical, speedFactor) => { };

	[SerializeField, Range(0f, 0.5f)]
	private float normalMoveSpeed = 0.05f;

	[SerializeField, Range(1f, 3f)]
	private float runSpeedMultiplier = 2f;

	private float runSpeed => normalMoveSpeed * runSpeedMultiplier;
	private float speedAnimFactor = 0.5f;
	private float moveFactor = 1;
	private bool movementEnabled = true;

	private CharacterController _charController;
	private CharacterController charController => _charController == null ? _charController = GetComponent<CharacterController>() : _charController;

	public void EnableMovement() {
		movementEnabled = true;
	}
	
	public void DisableMovement() {
		movementEnabled = false;
	}

	public void SetMoveFactor(float newMoveFactor = 1f) {
		moveFactor = newMoveFactor;
	}

	public void Move(UserInputHandler.MoveDir dir, bool running = false) {
		switch (dir) {
			case UserInputHandler.MoveDir.none:
				DontMove();
				break;
			case UserInputHandler.MoveDir.W:
				MoveW(running);
				break;
			case UserInputHandler.MoveDir.S:
				MoveS(running);
				break;
			case UserInputHandler.MoveDir.A:
				MoveA(running);
				break;
			case UserInputHandler.MoveDir.D:
				MoveD(running);
				break;
			case UserInputHandler.MoveDir.WD:
				MoveWD(running);
				break;
			case UserInputHandler.MoveDir.WA:
				MoveWA(running);
				break;
			case UserInputHandler.MoveDir.SD:
				MoveSD(running);
				break;
			case UserInputHandler.MoveDir.SA:
				MoveSA(running);
				break;
			default: 
				DontMove();
				break;
		}
	}

	public void MoveW(bool run = false) { MoveDir(transform.forward * (run ? runSpeed : normalMoveSpeed), 0f, 1f); }

	public void MoveS(bool run = false) { MoveDir(-transform.forward * (run ? runSpeed : normalMoveSpeed), 0f, -1f); }

	public void MoveA(bool run = false) { MoveDir(-transform.right * (run ? runSpeed : normalMoveSpeed), -1f, 0f); }

	public void MoveD(bool run = false) { MoveDir(transform.right * (run ? runSpeed : normalMoveSpeed), 1f, 0f); }

	public void MoveWA(bool run = false) { MoveDir(Vector3.Normalize(-transform.right + transform.forward) * (run ? runSpeed : normalMoveSpeed), -1f, 1f); }

	public void MoveWD(bool run = false) { MoveDir(Vector3.Normalize(transform.right + transform.forward) * (run ? runSpeed : normalMoveSpeed), 1f, 1f); }

	public void MoveSA(bool run = false) { MoveDir(Vector3.Normalize(-transform.right + -transform.forward) * (run ? runSpeed : normalMoveSpeed), -1f, -1f); }

	public void MoveSD(bool run = false) { MoveDir(Vector3.Normalize(transform.right + -transform.forward) * (run ? runSpeed : normalMoveSpeed), 1f, -1f); }

	public void DontMove() { MoveDir(Vector3.zero, 0f, 0f); }

	private void MoveDir(Vector3 dir, float xAnim, float yAnim) {
		if (!movementEnabled) return;
		
		charController.Move(dir * moveFactor * Time.timeScale);
		float currentSpeed = dir.magnitude;
		speedAnimFactor = Mathf.Lerp( speedAnimFactor, Mathf.InverseLerp(normalMoveSpeed, runSpeed, currentSpeed), 0.1f);
		OnMovementRequested(xAnim, yAnim, speedAnimFactor);
	}
}