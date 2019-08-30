using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementComponent : MonoBehaviour {

	public ControlledBy controlledBy = ControlledBy.Player;

	[SerializeField]
	private float moveSpeed;

	[SerializeField, Range(0f, 2f)]
	private float _rotationSmoothFactor = 0.6f;

	private CharacterController _charController;

	private CharacterController charController => _charController == null ? _charController = GetComponent<CharacterController>() : _charController;

	private Animator _animator;

	private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

	public CameraOrbit cameraOrbit;

	private Transform cameraRot => GM.instance.camera.gameObject.transform;

	private Vector3 moveDir;

	private void Update() {
		if (controlledBy == ControlledBy.AI)
			return;

		moveDir = Vector3.zero;

		if (Input.GetKey(KeyCode.W)) {
			moveDir += cameraRot.forward;
		}

		if (Input.GetKey(KeyCode.S)) {
			moveDir += cameraRot.forward * -1;
		}

		if (Input.GetKey(KeyCode.A)) {
			moveDir += cameraRot.right * -1;
		}

		if (Input.GetKey(KeyCode.D)) {
			moveDir += cameraRot.right;
		}

		if (moveDir != Vector3.zero)
			Move(moveDir);

		animator.SetBool("movingForward", Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S));
	}

	private void Move(Vector3 dir) {
		dir = Vector3.Normalize(new Vector3(dir.x, 0f, dir.z));
		transform.TransformDirection(dir);
		charController.Move(dir * moveSpeed);
		var targetRot = Quaternion.LookRotation(dir);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, _rotationSmoothFactor);
	}
}