using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraTransformManager : MonoBehaviour {

	public Transform transformToFollow;
	private Camera _managedCamera;
	private Camera managedCamera => _managedCamera == null ? _managedCamera = GetComponent<Camera>() : _managedCamera;

	public Vector3 camOffset = new Vector3();

	[Range(0, 1)]
	public float cameraPositionSensitivity = 1;

	private void Start() {
		camOffset = transform.position - transformToFollow.position;
	}

	private void LateUpdate() {
		managedCamera.transform.position = Vector3.Slerp(managedCamera.transform.position, transformToFollow.position, cameraPositionSensitivity);
	}
}