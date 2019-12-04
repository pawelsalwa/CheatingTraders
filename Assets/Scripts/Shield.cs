using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : WeaponTarget {

	public Action OnShieldImpacted = () => { };

    public Material damageTakingMaterial;

    public SkinnedMeshRenderer meshRenderer;
    [Range(0 , 500)]
    public int materialchangeMiliseconds = 80;
	protected override void OnHitReceived(int weaponDamage) {
		TakeImpactFromBlock();
	}

	private void TakeImpactFromBlock() {
		OnShieldImpacted();
	}
	

    private async void AnimateMaterialColorAsync() {
        // meshRenderer.material.color = Color.red;
        // await Task.Delay(materialchangeMiliseconds);
        // meshRenderer.material.color = Color.white;
    }
	
}
