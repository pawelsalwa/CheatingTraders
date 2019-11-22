using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class BodyTarget : WeaponTarget {

    public event Action<int> OnDamageTaken = (damage) => { };

    public Material damageTakingMaterial;

    public SkinnedMeshRenderer meshRenderer;
    public bool isTargettable = true;

    [Range(0 , 500)]
    public int materialchangeMiliseconds = 80;

    public void TakeDamage(int damage) {
        if (!isTargettable) return;
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
