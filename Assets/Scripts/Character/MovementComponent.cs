using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementComponent : NetworkBehaviour {

	[SerializeField, Range(0f, 0.5f)]
	private float moveSpeed;
	// private float runSpeed => moveSpeed * 2;

	[SerializeField, Range(1f, 3f)]
	private float runSpeedMultiplier;
	private float runSpeed => moveSpeed * runSpeedMultiplier;

	[SerializeField]
	private float currentSpeed;		
	
	[SerializeField]
	private float speedAnimFactor;		

	[SerializeField, Range(0f, 2f)]
	private float _rotationSmoothFactor = 0.6f;

	private CharacterController _charController;
	private CharacterController charController => _charController == null ? _charController = GetComponent<CharacterController>() : _charController;

	private Animator _animator;
	protected Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

	public float animSensitivity = 0.4f;

	private float oldAngle;
	private float newAngle;

	public void MoveW(bool run = false) { MoveDir(transform.forward * (run ? runSpeed : moveSpeed)); }

	public void MoveS(bool run = false) { MoveDir(-transform.forward * (run ? runSpeed : moveSpeed)); }

	public void MoveA(bool run = false) { MoveDir(-transform.right * (run ? runSpeed : moveSpeed)); }

	public void MoveD(bool run = false) { MoveDir(transform.right * (run ? runSpeed : moveSpeed)); }

	public void MoveWA(bool run = false) { MoveDir(Vector3.Normalize(-transform.right + transform.forward) * (run ? runSpeed : moveSpeed)); }

	public void MoveWD(bool run = false) { MoveDir(Vector3.Normalize(transform.right + transform.forward) * (run ? runSpeed : moveSpeed)); }

	public void MoveSA(bool run = false) { MoveDir(Vector3.Normalize(-transform.right + -transform.forward) * (run ? runSpeed : moveSpeed)); }

	public void MoveSD(bool run = false) { MoveDir(Vector3.Normalize(transform.right + -transform.forward) * (run ? runSpeed : moveSpeed)); }

	public void DontMove() { MoveDir(Vector3.zero); }

	private void MoveDir(Vector3 dir) {
		OnMoveRequested();
		currentSpeed = dir.magnitude;
		animator.SetBool("moving", dir != Vector3.zero);

		speedAnimFactor = Mathf.Lerp( speedAnimFactor, Mathf.InverseLerp(moveSpeed, runSpeed, currentSpeed), 0.1f);
		animator.SetLayerWeight(0, 1 - speedAnimFactor);
		animator.SetLayerWeight(1, speedAnimFactor);

		Vector3 lastPos = transform.position;
		charController.Move(dir);

		newAngle = Vector3.SignedAngle(transform.forward, transform.position - lastPos, Vector3.up);
		newAngle = Mathf.Repeat(Mathf.LerpAngle(oldAngle + 180f, newAngle + 180f, animSensitivity), 360f) - 180f;

		// angle = 0 moves forward, angle = 90 moves right angle = -90 moves left
		animator.SetFloat("movingAngle", newAngle); //, 0.1f, Time.deltaTime);

		oldAngle = newAngle;
		OnMoveRequested();
	}

//  ---Networking---

	protected virtual void OnMoveRequested() { }
}