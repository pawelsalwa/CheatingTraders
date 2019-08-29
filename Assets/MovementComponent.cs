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

	private Transform cameraRot => GM.instance.camera.gameObject.transform;

	private Vector3 moveDir = new Vector3();

	public void Init() { }

	private void RotateCharacter(Quaternion rotation) {

//		Vector3 newRotat = rotation.eulerAngles;
//		transform.rotation = Quaternion.Euler(newRotat.x, newRotat.y, newRotat.z);
	}

	private void Update() {
		moveDir = Vector3.zero;

		if (Input.GetKey(KeyCode.W)) {
			moveDir += cameraRot.forward;
			Move(moveDir);
		}
		
		if (Input.GetKey(KeyCode.S)) {
			moveDir += cameraRot.forward * -1;
			Move(moveDir);
		}
		
		if (Input.GetKey(KeyCode.A)) {
			moveDir += cameraRot.right * -1;
			Move(moveDir);
		}
		
		if (Input.GetKey(KeyCode.D)) {
			moveDir += cameraRot.right;
			Move(moveDir);
		}

		animator.SetBool("movingForward", Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S));
	}

	private void Move(Vector3 dir) {
		dir = Vector3.Normalize(new Vector3(dir.x, 0f, dir.z));
		transform.TransformDirection(dir);
		charController.Move(dir * moveSpeed);
		transform.rotation = Quaternion.LookRotation(dir);
	}
}