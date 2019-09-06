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
}