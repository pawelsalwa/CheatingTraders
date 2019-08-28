using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraTransformManager : MonoBehaviour {

	public Transform transformToFollow;
	private Camera _managedCamera;
	private Camera managedCamera => _managedCamera == null ? _managedCamera = GetComponent<Camera>() : _managedCamera;


	public float speed;
	public float maxSpeed = 0.1f;
	public float acceleration;
	public float lastSpeed;

	public float rotationSpeed;

	public Vector3 lastPosition = Vector3.zero;
	public Vector3 camOffset = new Vector3();

	[Range(0, 1)]
	public float SmoothFactor = 0.4f;

	private void Start() {
		camOffset = transform.position - transformToFollow.position;
	}
	private void LateUpdate() {
		managedCamera.transform.position = Vector3.Slerp(managedCamera.transform.position, transformToFollow.position, SmoothFactor);
		Vector3 newPos = transformToFollow.position + camOffset;
		transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
	}

	private void FixedUpdate() {
		speed = (transform.position - lastPosition).magnitude;
		lastPosition = transform.position;
		acceleration = speed - lastSpeed;
		lastSpeed = speed;
	}
}

		// Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up);
		// camOffset = camTurnAngle * camOffset;
		// UnityEngine.Debug.Log(Input.GetAxis("Mouse X"));
		// transform.LookAt(transformToFollow);

		// maxSpeed = maxSpeed < speed ? speed : maxSpeed;

		
	// private float fov {
	// 	get => managedCamera.fieldOfView;
	// 	set => managedCamera.fieldOfView = value;
	// }