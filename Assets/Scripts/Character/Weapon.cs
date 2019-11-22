using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

///<summary> Deals damage if its attached collider collides with <ref>AttackTarget</ref> </summary>
[RequireComponent(typeof(Collider))]
public class Weapon : MonoBehaviour {
    
    public event Action<BodyTarget, int> OnDamageDealt = (attTarget, damageDealt) => { };
    public event Action<Shield> OnShieldEncountered = (shield) => { };

    public int damage = 10;

    [Header("Should contain AttackTarget and Shield of character wielding this weapon :)")]
    public WeaponTarget[] ignoredBodyTargets;

//    private Collider _collider;
//	  public Collider collider => _collider == null ? _collider = GetComponent<Collider>() : _collider;

    private bool hasDealtDamage = false;

    private readonly Dictionary<BodyTarget, bool> targetToHasTakenDamage = new Dictionary<BodyTarget, bool>();

    [SerializeField]
    private List<BodyTarget> targets = new List<BodyTarget>();

    public void DealDamageIfFoundTarget() {
        if (targets.Count == 0)
            return;
        
        foreach (var target in targets) {
            if (targetToHasTakenDamage.ContainsKey((BodyTarget)target) && targetToHasTakenDamage[(BodyTarget)target]) 
                continue;
            
            if (!targetToHasTakenDamage.ContainsKey((BodyTarget)target))
                targetToHasTakenDamage.Add((BodyTarget)target, false);
            
            target.TakeDamage(damage);
            targetToHasTakenDamage[target] = true;
            OnDamageDealt(target, damage);
        }
    }

    public void StopDealingDamage() {
        ResetAllTargets();
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log("huj");
    }
    
    private void OnTriggerEnter(Collider other) {
        var newAttTarget = other.gameObject.GetComponent<WeaponTarget>();

        if (ignoredBodyTargets.Any(x => x == newAttTarget))      
            return;

        if (newAttTarget is Shield) {
            OnShieldEncountered(newAttTarget as Shield);
            Debug.Log("shield encountered");
            (newAttTarget as Shield).TakeImpactFromBlock();
        }

        if (newAttTarget is BodyTarget) {
            targets.Add(newAttTarget as BodyTarget);
        }
	}

    private void OnTriggerExit(Collider other) {
        var lastCollider = other.gameObject.GetComponent<WeaponTarget>();

        if (ignoredBodyTargets.Any(x => x == lastCollider))      
            return;

        if (lastCollider is BodyTarget && targets.Contains(lastCollider))
            targets.Remove(lastCollider as BodyTarget);
	}

    private void ResetAllTargets() {
        var asd = new List<BodyTarget>();
        foreach (var target in targetToHasTakenDamage) 
            asd.Add(target.Key);
            
        for (int i = 0; i < targetToHasTakenDamage.Count; i++)
            targetToHasTakenDamage[asd[i]] = false;
    }
}
