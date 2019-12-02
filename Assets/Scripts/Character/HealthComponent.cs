using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthComponent : MonoBehaviour {

	public bool isHpBelowZero => hp < 0;

	public int hp = 100;
	
	public HealthComponent TakeDamage(int damage) {
		hp -= damage;
		return this;
	}

}