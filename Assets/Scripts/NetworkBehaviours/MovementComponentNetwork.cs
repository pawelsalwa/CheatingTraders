using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementComponentNetwork : MovementComponent {

	private Vector3 lastPos;

//	protected override void OnMoveRequested() {
//		SendTransformToServer();
//	}

	protected override void OnMoveComputed() {
		SendTransformToServer();
	}

	private void SendTransformToServer() {
		if (isLocalPlayer)
			CmdSentTransformToServer(transform.position);
	}

	[Command]
	private void CmdSentTransformToServer(Vector3 clientPos) {
		RpcSetPosition(clientPos);
	}

	[ClientRpc]
	private void RpcSetPosition(Vector3 fromServPos) {
		if (isLocalPlayer) return;

		// ogarniamy animacje:
		lastPos = transform.position;
		Vector3 lastForward = transform.forward;
		float angle = Vector3.SignedAngle(lastForward, fromServPos - lastPos, Vector3.up);

		transform.position = fromServPos;

		if ((lastPos - fromServPos).magnitude < 0.01f) {
			animator.SetBool("moving", false);
		} else {
			animator.SetBool("moving", true);
			animator.SetFloat("movingAngle", angle);
		}
	}


// 	private void Update() {
//		if(isLocalPlayer)
//			CmdSetPosition(transform.position);
//	}
//	
//	[Command]
//	private void CmdSetPosition(Vector3 asd) {
//		RpcSetPosition(asd);
//	}
//
//	[ClientRpc]
//	private void RpcSetPosition(Vector3 asd) {
//		if (!isLocalPlayer)
//			transform.position = asd;
//	}


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