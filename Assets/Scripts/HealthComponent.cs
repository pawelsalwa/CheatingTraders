using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthComponent : MonoBehaviour {

    public event Action OnHpDropBelowZero = () => {};
	
	public AttackTarget attTarget;

	public int hp = 100;

	public void Awake() {
		attTarget.OnDamageTaken += (damage) => TakeDamage(damage);
	}

	private void TakeDamage(int damage) {
		hp -= damage;

		if (hp <= 0) 
			OnHpDropBelowZero();
	}
}