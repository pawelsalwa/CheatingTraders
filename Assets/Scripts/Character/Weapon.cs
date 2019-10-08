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
    public BodyTarget[] ignoredBodyTargets;

//    private Collider _collider;
//	  public Collider collider => _collider == null ? _collider = GetComponent<Collider>() : _collider;

    private bool hasDealtDamage = false;

    private Dictionary<BodyTarget, bool> targetToHasTakenDamage = new Dictionary<BodyTarget, bool>();

    [SerializeField]
    private List<AttackTarget> attTargets = new List<AttackTarget>();

    public void DealDamageIfFoundTarget() {
        if (attTargets.Count == 0)
            return;
        
        foreach (var target in attTargets) {
            if (targetToHasTakenDamage.ContainsKey(target) && targetToHasTakenDamage[target]) 
                continue;
            
            if (!targetToHasTakenDamage.ContainsKey(target))
                targetToHasTakenDamage.Add(target, false);
            
            target.TakeDamage(damage);
            targetToHasTakenDamage[target] = true;
            OnDamageDealt(target, damage);
        }
    }

    public void StopDealingDamage() {
        ResetAllTargets();
    }
	
	private void OnTriggerEnter(Collider other) {
        var newAttTarget = other.gameObject.GetComponent<AttackTarget>();

        if (ignoredBodyTargets.Any(x => x == newAttTarget))      
            return;

        if (newAttTarget is Shield) {
            OnShieldEncountered(newAttTarget as Shield);
        }

        attTargets.Add(newAttTarget);
	}

    private void OnTriggerExit(Collider other) {
        var lastCollider = other.gameObject.GetComponent<BodyTarget>();

        if (ignoredBodyTargets.Any(x => x == lastCollider))      
            return;

        if (attTargets.Contains(lastCollider))
            attTargets.Remove(lastCollider);
	}

    private void ResetAllTargets() {
        var asd = new List<BodyTarget>();
        foreach (var target in targetToHasTakenDamage) 
            asd.Add(target.Key);
            
        for (int i = 0; i < targetToHasTakenDamage.Count; i++)
            targetToHasTakenDamage[asd[i]] = false;
    }
}
