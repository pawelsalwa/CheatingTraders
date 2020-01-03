using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class BodyTarget : WeaponTarget {

    public event Action<float> OnDamageTaken = (damage) => { };

    public Material damageTakingMaterial;

    public SkinnedMeshRenderer meshRenderer;

    [Range(0 , 500)]
    public int materialchangeMiliseconds = 80;

    public bool enableDebugs = false;

    protected override void OnHitReceived(float weaponDamage) {
        TakeDamage(weaponDamage);
    }

    private void TakeDamage(float damage) {
        if(enableDebugs)
            Debug.Log("damage taken " + damage, gameObject);
        OnDamageTaken(damage);
        AnimateMaterialColorAsync();
    }

    private async void AnimateMaterialColorAsync() {
        meshRenderer.material.color = Color.red;
        await Task.Delay(materialchangeMiliseconds);
        meshRenderer.material.color = Color.white;
    }
}
