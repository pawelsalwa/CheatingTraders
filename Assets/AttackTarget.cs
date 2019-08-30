using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

///<summary> Component being able to take damage </summary>
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class AttackTarget : MonoBehaviour {

    public event Action<int> OnDamageTaken = (damage) => { };

    public Material damageTakingMaterial;

    public void TakeDamage(int damage) {
        Debug.Log("damage taken " + damage, gameObject);
        OnDamageTaken(damage);
    }
}
