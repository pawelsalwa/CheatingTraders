using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementComponent : MonoBehaviour, Initable {

	[SerializeField]
	private float moveSpeed;

	private CharacterController _charController;

	private CharacterController charController => _charController == null ? _charController = GetComponent<CharacterController>() : _charController;

	private Animator _animator;

	private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

	public CameraOrbit cameraOrbit;

	public void Init() { }

	private void RotateCharacter(Quaternion rotation) {

//		Vector3 newRotat = rotation.eulerAngles;
//		transform.rotation = Quaternion.Euler(newRotat.x, newRotat.y, newRotat.z);
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

		transform.rotation = GM.instance.cinemachineFreeLook.gameObject.transform.rotation;
	}
}