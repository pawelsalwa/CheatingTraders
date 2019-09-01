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
	
	public void MoveW() {
		SetSingleAnimKey(W);
		charController.Move(transform.forward * moveSpeed);
	}
	
	public void MoveS() {
		SetSingleAnimKey(S);
		charController.Move(-transform.forward * moveSpeed);
	}
	
	public void MoveA() {
		SetSingleAnimKey(A);
		charController.Move(-transform.right * moveSpeed);
	}
	
	public void MoveD() {
		SetSingleAnimKey(D);
		charController.Move(transform.right * moveSpeed);
	}
	
	public void MoveWA() {
		SetSingleAnimKey(WA);
		charController.Move(Vector3.Normalize(-transform.right + transform.forward) * moveSpeed);
	}
	
	public void MoveWD() {
		SetSingleAnimKey(WD);
		charController.Move(Vector3.Normalize(transform.right + transform.forward) * moveSpeed);
	}
	
	public void MoveSA() {
		SetSingleAnimKey(SA);
		charController.Move(Vector3.Normalize(-transform.right + -transform.forward) * moveSpeed);
	}
	
	public void MoveSD() {
		SetSingleAnimKey(WD);
		charController.Move(Vector3.Normalize(transform.right + -transform.forward) * moveSpeed);
	}
	
	public void DontMove() {
		SetSingleAnimKey(null);
//		charController.Move(Vector3.Normalize(transform.right + transform.forward) * moveSpeed);
	}

	private void SetSingleAnimKey(string key) {
		foreach (var xd in new List<string> {W,A,S,D,WA,WD,SA,SD}) animator.SetBool(xd, xd == key); // slychac bol dupy lamusow
	}

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