using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> Component being able to collide with weapon </summary>
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public abstract class WeaponTarget : MonoBehaviour {

	public event Action OnDestroyCalled = () => { };

	public bool isTargettable = true;

	public void ReceiveWeaponHit(float weaponDamage) {
		if (!isTargettable) return;

		OnHitReceived(weaponDamage);
	}

	protected abstract void OnHitReceived(float weaponDamage);

	private void OnDestroy() {
		OnDestroyCalled();
	}
}