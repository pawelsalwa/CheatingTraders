using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDebugger : MonoBehaviour
{
   private void OnCollisionEnter(Collision other) {
      Debug.Log("OnCollisionEnter" + other.gameObject.name);
   }

   private void OnTriggerEnter(Collider other) {
      Debug.Log("trigger");
   }
}
