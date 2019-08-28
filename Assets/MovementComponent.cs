using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementComponent : MonoBehaviour {

	[SerializeField]
	private float moveSpeed;

	private CharacterController _charController;
	private CharacterController charController {
		get {
			if (_charController == null)
				_charController = GetComponent<CharacterController>();
			return _charController;
		}
	}

		private Animator _animator;
	private Animator animator {
		get {
			if (_animator == null)
				_animator = GetComponent<Animator>();
			return _animator;
		}
	}


	private void Update() {
		if (Input.GetKey(KeyCode.W)) {
			var moveDir = new Vector3(0, 0, 1);
			transform.TransformDirection(moveDir);
			charController.Move(moveDir * moveSpeed);
		}
		animator.SetBool("movingForward", Input.GetKey(KeyCode.W));

		if (Input.GetKey(KeyCode.S)) {
			var moveDir = new Vector3(0, 0, -1);
			transform.TransformDirection(moveDir);
			charController.Move(moveDir * moveSpeed);
		}
		animator.SetBool("movingBackward", Input.GetKey(KeyCode.S));

		if (Input.GetKey(KeyCode.D)) {
			var moveDir = new Vector3(1, 0, 0);
			transform.TransformDirection(moveDir);
			charController.Move(moveDir * moveSpeed);
		}

		if (Input.GetKey(KeyCode.A)) {
			var moveDir = new Vector3(-1, 0, 0);
			transform.TransformDirection(moveDir);
			charController.Move(moveDir * moveSpeed);
		}
	}
}