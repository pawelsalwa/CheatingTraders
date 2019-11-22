using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : WeaponTarget {

	public Action OnShieldImpacted = () => { };

	public void TakeImpactFromBlock() {
		OnShieldImpacted();
	}
	
}
