using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthComponent : MonoBehaviour {

	public event Action<float> OnHPChanged = (newHP) => { };

	public HPBar hpBar;
	
	public bool isHpBelowZero => Mathf.Approximately( currentHP, 0f);

	private float maxHP => GM.projectConstants.unit.hp.maxHP;
	private float hpRegenPerSec => GM.projectConstants.unit.hp.hpRegenPerSec;
	private float regainHPRegenAfterLossTimeout => GM.projectConstants.unit.hp.regainhpRegenAfterLossTimeout;

	private bool regenEnabled = true;
	
	[SerializeField, Range(0f, 100f)]
	private float _currentHP;
	private float currentHP {
		get => Mathf.Clamp(_currentHP, 0, maxHP);
		set {
			value = Mathf.Clamp(value, 0, maxHP);
			
			if (Mathf.Approximately(value, _currentHP))
				return;

			_currentHP = value;
			hpBar.SetHpPercentage(_currentHP / maxHP);
			OnHPChanged(_currentHP);
		}
	}

	public void TakeDamage(float damage) {
		currentHP -= damage;
		
		regenEnabled = false;
		
		CancelInvoke(nameof(RegainHPRegen));
		Invoke(nameof(RegainHPRegen), regainHPRegenAfterLossTimeout);
	}

	public void InitAsPlayer() {
		hpBar.gameObject.SetActive(false);
	}

	private void RegainHPRegen() {
		regenEnabled = true;
	}

	private void Update() {
		if (regenEnabled) {
			RegenHP();
		}
	}

	private void RegenHP() {
		currentHP += Time.deltaTime * hpRegenPerSec;
	}

	private void Awake() {
		currentHP = maxHP;
	}

}