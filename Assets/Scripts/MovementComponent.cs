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

	public void MoveS() { MoveDir(-1.01f * transform.forward * moveSpeed); }

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
		float angle = Vector3.SignedAngle(transform.forward, transform.position - lastPos, Vector3.up);
		
		// angle = 0 moves forward, angle = 90 moves right angle = -90 moves left
		animator.SetFloat("movingAngle", angle, 0.1f, Time.deltaTime);  
		OnMoveRequested();
	}

	private void SetSingleAnimKey(string key) {
		foreach (var xd in new List<string> {W, A, S, D, WA, WD, SA, SD}) animator.SetBool(xd, xd == key); // slychac bol dupy lamusow
	}

//  ---Networking---

	protected virtual void OnMoveRequested() { }
}