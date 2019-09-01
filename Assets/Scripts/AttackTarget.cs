using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

///<summary> Component being able to take damage </summary>
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class AttackTarget : MonoBehaviour {

    public event Action<int> OnDamageTaken = (damage) => { };

    public Material damageTakingMaterial;

    public SkinnedMeshRenderer[] meshRenderer;
    public bool isTargettable = true;

    public bool animateMaterial = true;
    public bool animateColorOnly = false;

    [Range(0 , 500)]
    public int materialchangeMiliseconds = 80;

    public void TakeDamage(int damage) {
        if (!isTargettable) return;
        Debug.Log("damage taken " + damage, gameObject);
        OnDamageTaken(damage);
        if (animateMaterial)
            AnimateMaterialAsync();
        
        if (animateColorOnly)
            AnimateMaterialColorAsync();
    }

    private async void AnimateMaterialColorAsync() {
        foreach (var xd in meshRenderer)
            xd.material.color = Color.red;
        await Task.Delay(materialchangeMiliseconds);
        foreach (var xd in meshRenderer)
            xd.material.color = Color.white;
    }
    
    private async void AnimateMaterialAsync() {
        var tmp = meshRenderer[0].material;
        foreach (var xd in meshRenderer)
            xd.material = damageTakingMaterial;
        await Task.Delay(materialchangeMiliseconds);
        foreach (var xd in meshRenderer)
            xd.material = tmp;
    }
}
