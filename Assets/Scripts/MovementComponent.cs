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

	private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

	[Header("AnimatorKeys")]
//	public string noMove;
	public string W;

	public string S;
	public string A;
	public string D;
	public string WA;
	public string WD;
	public string SA;
	public string SD;

	public void MoveW() { MoveDir(transform.forward * moveSpeed); }

	public void MoveS() { MoveDir(-transform.forward * moveSpeed); }

	public void MoveA() { MoveDir(-transform.right * moveSpeed); }

	public void MoveD() { MoveDir(transform.right * moveSpeed); }

	public void MoveWA() { MoveDir(Vector3.Normalize(-transform.right + transform.forward) * moveSpeed); }

	public void MoveWD() { MoveDir(Vector3.Normalize(transform.right + transform.forward) * moveSpeed); }

	public void MoveSA() { MoveDir(Vector3.Normalize(-transform.right + -transform.forward) * moveSpeed); }

	public void MoveSD() { MoveDir(Vector3.Normalize(transform.right + -transform.forward) * moveSpeed); }

	public void DontMove() { animator.SetBool("moving", false); }

	private void MoveDir(Vector3 dir) {
		animator.SetBool("moving", true);
		Vector3 lastPos = transform.position;
		charController.Move(dir);
		animator.SetFloat("movingAngle", Vector3.SignedAngle(transform.forward, transform.position - lastPos, Vector3.up), 0.1f, Time.deltaTime);
		Debug.Log("angle: " + Vector3.SignedAngle(transform.forward, transform.position - lastPos, Vector3.up));
		OnMoveRequested();
	}

	private void SetSingleAnimKey(string key) {
		foreach (var xd in new List<string> {W, A, S, D, WA, WD, SA, SD}) animator.SetBool(xd, xd == key); // slychac bol dupy lamusow
	}

//  ---Networking---

	protected virtual void OnMoveRequested() { }

//	public void MoveRight(Vector3 dir) {
//		animator.SetBool("strafeD", dir != Vector3.zero);
//		if(dir == Vector3.zero)
//			return;
//		
//		dir = Vector3.Normalize(new Vector3(dir.x, 0f, dir.z));
//		charController.Move(dir * moveSpeed);
//	}
//
//	public void Move(Vector3 dir) {
//		animator.SetBool("movingForward", dir != Vector3.zero);
//		if(dir == Vector3.zero)
//			return;
//		
//		dir = Vector3.Normalize(new Vector3(dir.x, 0f, dir.z));
//		transform.TransformDirection(dir);
//		charController.Move(dir * moveSpeed);
//		var targetRot = Quaternion.LookRotation(dir);
//		transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, _rotationSmoothFactor);
//	}
}