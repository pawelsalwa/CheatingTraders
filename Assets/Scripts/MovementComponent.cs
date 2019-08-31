using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementComponent : MonoBehaviour {

	[SerializeField, Range(0f, 0.5f)]
	private float moveSpeed;

	[SerializeField, Range(0f, 2f)]
	private float _rotationSmoothFactor = 0.6f;

	[Header("Animator keys")]
	public string strafeLeftKey = "strafeLeft";
	public string strafeRightKey = "strafeRight";
	public string walkForwardKey = "walkForward";
	public string walkBackwardKey = "walkBackward";
	
	private CharacterController _charController;

	private CharacterController charController => _charController == null ? _charController = GetComponent<CharacterController>() : _charController;

	private Animator _animator;

	private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

	private Transform cameraRot => GM.instance.camera.gameObject.transform;

	public void NoMove() {
		animator.SetBool(walkForwardKey, false);
		animator.SetBool(walkBackwardKey, false);
		animator.SetBool(strafeLeftKey, false);
		animator.SetBool(strafeRightKey, false);
	}

	public void WalkForward() {
		Vector3 dir = transform.forward;
		animator.SetBool(walkForwardKey, true);
//		charController.Move(dir * moveSpeed);
	}
	
	public void WalkBackWard() {
		Vector3 dir = transform.forward;
		animator.SetBool(walkBackwardKey, true);
//		charController.Move(dir * moveSpeed);
	}

	public void StrafeLeft() {
		animator.SetBool(strafeLeftKey, true);
	}
	
	public void StrafeRight() {
		animator.SetBool(strafeRightKey, true);
	}
	
	public void Move(Vector3 dir) {
		animator.SetBool(walkForwardKey, dir != Vector3.zero);
		if(dir == Vector3.zero)
			return;
		
		dir = Vector3.Normalize(new Vector3(dir.x, 0f, dir.z));
		transform.TransformDirection(dir);
		charController.Move(dir * moveSpeed);
//		var targetRot = Quaternion.LookRotation(dir);
//		transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, _rotationSmoothFactor);
	}
}