using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour {
	
	
	public ControlledBy controlledBy = ControlledBy.Player;
	
	private void OnTriggerEnter(Collider other) {

//		var asd = other.GetComponent<>();
//		
//		if()
		
		Debug.Log("sword entered " + other.gameObject.name);
	}
}
