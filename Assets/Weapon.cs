using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(Collider))]
public class Weapon : MonoBehaviour {
    public event Action OnDamageDealt = () => {};

    public int damage = 10;

    public AttackTarget[] ignoredAttackTargets;

    private Collider _collider;
	public Collider collider => _collider == null ? _collider = GetComponent<Collider>() : _collider;

    private bool hasTarget = false;

    private AttackTarget attTarget = null;

    public void DealDamageIfFoundTarget() {
        if (attTarget == null) 
            return;        

        attTarget.TakeDamage(damage);
        attTarget = null;
        hasTarget = false;
        OnDamageDealt();
    }
	
	private void OnTriggerEnter(Collider other) {
        attTarget = other.gameObject.GetComponent<AttackTarget>();

        if (attTarget == null) 
            return;       

        if (ignoredAttackTargets.Any(x => x == attTarget)){
            attTarget = null;
            return;
        }

        if (hasTarget)
            return;

        hasTarget = true;		
	}

    private void OnTriggerExit(Collider other) {
        attTarget = null;
        hasTarget = false;
	}
}
