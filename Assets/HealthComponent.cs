using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class HealthComponent : MonoBehaviour {
	
	
	public ControlledBy controlledBy = ControlledBy.Player;
	
	private void OnTriggerEnter(Collider other) {
		
//		if (controlledBy == other.contro) return;
		
// 		var sword = other.gameObject.GetComponent<SwordCollider>();
// 		if (sword == null)
// 			return;

		// Debug.Log("TakeDamage" + gameObject.name);
	}
}