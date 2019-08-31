using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;  
using Cinemachine;

public class SetupLocalPlayer : NetworkBehaviour {

    public Transform cameraOrbitCenter;
    public Transform cameraFollowTransform;

    private UserInputHandler _userInputHandler;
	private UserInputHandler userInputHandler => _userInputHandler == null ? _userInputHandler = GetComponent<UserInputHandler>() : _userInputHandler;

    void Start() {
        if (isLocalPlayer) {
            userInputHandler.enabled = true;
            GM.instance.cinemachineFreeLook.m_Follow = cameraFollowTransform;
            GM.instance.cinemachineFreeLook.m_LookAt = cameraOrbitCenter;
        }
    }
}
