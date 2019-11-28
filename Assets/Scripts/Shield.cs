using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : WeaponTarget {

	public Action OnShieldImpacted = () => { };

	protected override void OnHitReceived(int weaponDamage) {
		TakeImpactFromBlock();
	}

	private void TakeImpactFromBlock() {
		OnShieldImpacted();
	}
	
}
