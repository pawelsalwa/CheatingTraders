using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror;
using UnityEditor;
using UnityEngine;

public class BasicUnitNetwork : BasicUnit {

	protected override void Start() {
		base.Start();
		if (isLocalPlayer) {
			GM.instance._player = this;
			GM.instance.cinemachineFreeLook.m_Follow = cameraFollow;
			GM.instance.cinemachineFreeLook.m_LookAt = cameraOrbit;
			GM.instance.cinemachineFreeLook.enabled = true;
			
			userInputHandler.enabled = true;
			GetComponent<BotController>().enabled = false;

			base.OnDeath += GM.instance.youDiedMenu.Open;
		} else {
			userInputHandler.enabled = false;
			GetComponent<BotController>().enabled = false;
		}
	}
}