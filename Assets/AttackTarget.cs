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

    public SkinnedMeshRenderer meshRenderer;

    [Range(0 , 500)]
    public int materialchangeMiliseconds = 80;

    public void TakeDamage(int damage) {
        Debug.Log("damage taken " + damage, gameObject);
        AnimateMaterialColorAsync();
        OnDamageTaken(damage);
    }

    private async void AnimateMaterialColorAsync() {
        meshRenderer.material.color = Color.red;
        await Task.Delay(materialchangeMiliseconds);
        meshRenderer.material.color = Color.white;
    }
}
