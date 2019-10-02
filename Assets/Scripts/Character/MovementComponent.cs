using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementComponent : NetworkBehaviour {

	[SerializeField, Range(0f, 0.5f)]
	private float moveSpeed;

	[SerializeField, Range(0f, 2f)]
	private float _rotationSmoothFactor = 0.6f;

	private CharacterController _charController;
	private CharacterController charController => _charController == null ? _charController = GetComponent<CharacterController>() : _charController;

	private Animator _animator;
	protected Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

	public float animSensitivity = 0.4f;

	private float oldAngle;
	private float newAngle;

	public void MoveW() { MoveDir(transform.forward * moveSpeed); }

	public void MoveS() { MoveDir(-transform.forward * moveSpeed); }

	public void MoveA() { MoveDir(-transform.right * moveSpeed); }

	public void MoveD() { MoveDir(transform.right * moveSpeed); }

	public void MoveWA() { MoveDir(Vector3.Normalize(-transform.right + transform.forward) * moveSpeed); }

	public void MoveWD() { MoveDir(Vector3.Normalize(transform.right + transform.forward) * moveSpeed); }

	public void MoveSA() { MoveDir(Vector3.Normalize(-transform.right + -transform.forward) * moveSpeed); }

	public void MoveSD() { MoveDir(Vector3.Normalize(transform.right + -transform.forward) * moveSpeed); }

	public void DontMove() { MoveDir(Vector3.zero); }

	private void MoveDir(Vector3 dir) {
		OnMoveRequested();
		animator.SetBool("moving", dir != Vector3.zero);

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