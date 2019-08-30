using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

///<summary> Deals damage if its attached collider collides with <ref>AttackTarget</ref> </summary>
[RequireComponent(typeof(Collider))]
public class Weapon : MonoBehaviour {
    public event Action OnDamageDealt = () => {};

    public int damage = 10;

    [Header("Should contain AttackTarget of character wielding this weapon :)")]
    public AttackTarget[] ignoredAttackTargets;

    private Collider _collider;
	public Collider collider => _collider == null ? _collider = GetComponent<Collider>() : _collider;

    private bool hasDealtDamage = false;

    [SerializeField]
    private AttackTarget attTarget = null;

    public void DealDamageIfFoundTarget() {
        if (attTarget == null) 
            return;        

        if (hasDealtDamage)
            return;
        
        attTarget.TakeDamage(damage);
        hasDealtDamage = true;

        OnDamageDealt();
    }

    public void StopDealingDamage() {
        hasDealtDamage = false;
    }
	
	private void OnTriggerEnter(Collider other) {
        var newAttTarget = other.gameObject.GetComponent<AttackTarget>();

        if (ignoredAttackTargets.Any(x => x == newAttTarget))      
            return;

        attTarget = newAttTarget;
	}

    private void OnTriggerExit(Collider other) {
        var lastCollider = other.gameObject.GetComponent<AttackTarget>();

        if (ignoredAttackTargets.Any(x => x == lastCollider))      
            return;
        
        attTarget = null;
	}
}
