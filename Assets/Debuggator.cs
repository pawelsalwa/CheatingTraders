using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuggator : MonoBehaviour
{

	private void OnCollisionEnter(Collision other) {
		Debug.Log("OnCollisionEnter" + gameObject);
	}

	private void OnTriggerEnter(Collider other) {
		Debug.Log("OnTriggerEnter");
	}
	
}
