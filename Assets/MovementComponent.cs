using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementComponent : MonoBehaviour {

	[SerializeField, Range(0f, 0.5f)]
	private float moveSpeed;

	[SerializeField, Range(0f, 2f)]
	private float _rotationSmoothFactor = 0.6f;

	private CharacterController _charController;

	private CharacterController charController => _charController == null ? _charController = GetComponent<CharacterController>() : _charController;

	private Animator _animator;

	private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

	public void Move(Vector3 dir) {
		animator.SetBool("movingForward", dir != Vector3.zero);
		if(dir == Vector3.zero)
			return;
		
		dir = Vector3.Normalize(new Vector3(dir.x, 0f, dir.z));
		transform.TransformDirection(dir);
		charController.Move(dir * moveSpeed);
		var targetRot = Quaternion.LookRotation(dir);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, _rotationSmoothFactor);
	}
}