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
    [SerializeField] public bool enableDebugs = false;

    [Header("Should contain WeaponTarget and Shield of character wielding this weapon :)")]
    public WeaponTarget[] ignoredWeaponTargets;

    private bool dealingDamageEnabled = false;

    private readonly Dictionary<BodyTarget, bool> targetToHasTakenDamage = new Dictionary<BodyTarget, bool>();

    [SerializeField]
    private List<WeaponTarget> targets = new List<WeaponTarget>();
    
    public void StartDealingDamage() {
        if (enableDebugs)
            Debug.Log($"<color=blue>start dmg from anim {targets.Count} </color>");
        dealingDamageEnabled = true;
        DealDamageIfFoundTarget();
    }
    
    public void EndDealingDamage() {
        if (enableDebugs)
            Debug.Log($"<color=magenta>stop dmg from anim {targets.Count} </color>");
        dealingDamageEnabled = false;
        ResetAllTargets();
    }

    private void DealDamageIfFoundTarget() {
        if (!dealingDamageEnabled) return;
        RemoveNotValidTargets();
        
        foreach (var target in targets) {

            if (target is Shield) {
                ShieldEncountered(target as Shield);
                continue;                
            }

            if (target is BodyTarget) {
                BodyTargetEncountered(target as BodyTarget);
                continue;
            }            
        }
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

        if (enableDebugs)
            Debug.Log($"<color=orange>col entered {targets.Count} " + (dealingDamageEnabled ? @"dealing enabled" : @"dealing disabled") + "</color>\n" , newWeaponTarget.gameObject);

        DealDamageIfFoundTarget();
    }
    
    private void OnTriggerExit(Collider other) {
        var lastWeaponTarget = other.gameObject.GetComponent<WeaponTarget>();
        
        if (ignoredWeaponTargets.Any(x => x == lastWeaponTarget))      
            return;

        if (targets.Contains(lastWeaponTarget)) {
            targets.Remove(lastWeaponTarget);
            if (enableDebugs)
                Debug.Log($"<color=green>col exited {targets.Count} </color>", lastWeaponTarget.gameObject);
        }

        BodyTarget lastBodyTarget = other.gameObject.GetComponent<BodyTarget>();
        if (lastBodyTarget == null)
            return;
        
        if (targetToHasTakenDamage.ContainsKey(lastBodyTarget)) 
            targetToHasTakenDamage.Remove(lastBodyTarget);
	}

    private void ResetAllTargets() {
        var targetsList = new List<BodyTarget>(targetToHasTakenDamage.Keys);
        foreach (var target in targetsList)
            targetToHasTakenDamage[target] = false;
    }

    private void RemoveNotValidTargets() {
        for (int i = targets.Count - 1; i >= 0; i--)
             if (targets[i] == null || !targets[i].gameObject.activeSelf)
                 targets.Remove(targets[i]);
    }

    private void CheckDictionaryHealth() {
        List<BodyTarget> removals = new List<BodyTarget>();
        foreach( var item in targetToHasTakenDamage)
            if (item.Key == null) 
                removals.Add(item.Key);
                
        foreach (BodyTarget XD in removals) 
            targetToHasTakenDamage.Remove(XD);
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
            targetToHasTakenDamage.Add(bodyTarget, true);
        
        if (enableDebugs)
            Debug.Log($"<color=red>dealing dmg {targets.Count} </color>");
        
        bodyTarget.ReceiveWeaponHit(damage);
        OnDamageDealt(bodyTarget, damage);
    }
    
    private void FixedUpdate() {
        DealDamageIfFoundTarget();
    }

    private void DebugDic() {
        int i = 0;
        foreach (var item in targetToHasTakenDamage)
            Debug.Log($"<color=teal> object nr {i++} dealt= </color>" + targetToHasTakenDamage[item.Key], item.Key.gameObject);
    }
}
