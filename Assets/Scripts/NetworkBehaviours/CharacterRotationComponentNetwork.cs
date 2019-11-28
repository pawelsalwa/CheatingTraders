using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CharacterRotationComponentNetwork : CharacterRotationComponent {
//
//	private Vector3 lastRot;
//
//	protected override void OnRotateRequested() {
//		SendTransformToServer();
//	}
//
//	private void LateUpdate() {
//		SendTransformToServer();
//	}	
//
//	private void SendTransformToServer() {
//		if (isLocalPlayer) {
//			CmdSentTransformToServer(transform.rotation);
//		}
//	}
//
//	[Command]
//	private void CmdSentTransformToServer(Quaternion clientRot) {
//		RpcSetPosition(clientRot);
//	}
//
//	[ClientRpc]
//	private void RpcSetPosition(Quaternion clientRot) {
//		if (isLocalPlayer) return;
//
//		// ogarniamy animacje:
//		lastRot = transform.rotation.eulerAngles;
//		Vector3 newClientRot = clientRot.eulerAngles;
//		float clientRotationSpeed = lastRot.y - newClientRot.y;
//
//		AnimateRotation(clientRotationSpeed);
//		transform.rotation = clientRot;
//	}
}