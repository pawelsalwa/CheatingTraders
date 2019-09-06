using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementComponentNetwork : MovementComponent {

	private Vector3 lastPos;

	protected override void OnMoveRequested() {
		SendTransformToServer();
	}
	
	private void SendTransformToServer() {
		if(isLocalPlayer)
			CmdSentTransformToServer(transform.position);
	}

	[Command]
	private void CmdSentTransformToServer(Vector3 clientPos) {
		RpcSetPosition(clientPos);
	}

	[ClientRpc]
	private void RpcSetPosition(Vector3 fromServPos) {
		if (isLocalPlayer) return;

		lastPos = transform.position;
		
		
		transform.position = fromServPos;
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