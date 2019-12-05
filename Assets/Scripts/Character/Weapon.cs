using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

///<summary> Deals damage if its attached collider collides with <ref>WeaponTarget</ref> </summary>
[RequireComponent(typeof(Collider))]
public class Weapon : MonoBehaviour {
    
    public event Action<BodyTarget, int> OnDamageDealt = (attTarget, damageDealt) => { };
    public event Action<Shield> OnEnemyShieldEncounter = (shield) => { };

    [SerializeField] private int damage = 10;
    [SerializeField] private bool enableDebugs = false;

    [Header("Should contain WeaponTarget and Shield of character wielding this weapon :)")]
    public WeaponTarget[] ignoredWeaponTargets;

    private bool hasDealtDamage = false;

    private readonly Dictionary<WeaponTarget, bool> targetToHasTakenDamage = new Dictionary<WeaponTarget, bool>();

    [SerializeField]
    private List<WeaponTarget> targets = new List<WeaponTarget>();

    public void DealDamageIfFoundTarget() {
        RemoveDestroyedTargets();
        
        foreach (var target in targets) {

            if (target is Shield) {
                ShieldEncountered(target as Shield);
                return;                
            }

            if (target is BodyTarget) {
                BodyTargetEncountered(target as BodyTarget);
                return;
            }            
        }
    }

    public void StopDealingDamage() {
        ResetAllTargets();
    }
    
    private void OnTriggerEnter(Collider other) {
        var newWeaponTarget = other.gameObject.GetComponent<WeaponTarget>();

        if (ignoredWeaponTargets.Any(x => x == newWeaponTarget))      
            return;

        if (newWeaponTarget == null)
            return;

        if (targets.Contains(newWeaponTarget))
            return;

        targets.Add(newWeaponTarget);
	}

    private void OnTriggerExit(Collider other) {
        var lastWeaponTarget = other.gameObject.GetComponent<WeaponTarget>();
        
        if (ignoredWeaponTargets.Any(x => x == lastWeaponTarget))      
            return;

        if (targets.Contains(lastWeaponTarget))
            targets.Remove(lastWeaponTarget);
	}

    private void ResetAllTargets() {
        var asd = new List<WeaponTarget>();
        foreach (var target in targetToHasTakenDamage) 
            asd.Add(target.Key);
            
        for (int i = 0; i < targetToHasTakenDamage.Count; i++)
            targetToHasTakenDamage[asd[i]] = false;
    }

    private void RemoveDestroyedTargets() {
        for (int i = targets.Count - 1; i >= 0; i--)
             if (targets[i] == null)
                 targets.Remove(targets[i]);
    }

    private void ShieldEncountered(Shield shield) {
        if (enableDebugs)
            Debug.Log("shield encountered");
        OnEnemyShieldEncounter(shield);
        shield.ReceiveWeaponHit(0);
    }

    private void BodyTargetEncountered(BodyTarget bodyTarget) {
        if (targetToHasTakenDamage.ContainsKey(bodyTarget) && targetToHasTakenDamage[bodyTarget])
            return;
        
        if (!targetToHasTakenDamage.ContainsKey(bodyTarget))
            targetToHasTakenDamage.Add(bodyTarget, false);
        
        bodyTarget.ReceiveWeaponHit(damage);
        targetToHasTakenDamage[bodyTarget] = true;
        OnDamageDealt(bodyTarget, damage);
    }
}
