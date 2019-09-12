using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[Obsolete("Stary kod ;(")]
public class CameraOrbit : MonoBehaviour {

	public event Action<Quaternion> OnOrbit = newGlobalPosition => { };

	public Transform orbitCenter;

	public float turnSpeed = 4.0f;
	public float maxYRotation = 50f, minYRotation = -23f;
	private Vector3 offset;

	void Start() {
		offset = new Vector3(orbitCenter.position.x, orbitCenter.position.y, orbitCenter.position.z);
	}

	void LateUpdate() {
		if (GM.isAnyMenuOpened) return;
		offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
		offset = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * turnSpeed, Vector3.left) * offset;

		transform.position = orbitCenter.position + offset;
		transform.LookAt(orbitCenter.position);

		OnOrbit(transform.rotation);
	}
}