using UnityEngine;
using System;

public class StaminaComponent : MonoBehaviour {

	public event Action<float> OnStaminaChanged = (staminaChanged) => { };
	public event Action OnNotEnoughStaminaForAction = () => { };

	private float maxStamina => GM.projectConstants.unit.stamina.maxStamina;
	private float staminaRegenPerSec => GM.projectConstants.unit.stamina.staminaRegenPerSec;
	private float regainStaminaRegenAfterLossTimeout => GM.projectConstants.unit.stamina.regainStaminaRegenAfterLossTimeout;
	private float dodgeCost => GM.projectConstants.unit.stamina.dodgeStaminaCost;
	
//	[SerializeField] private float attackCost = 20f;
//	[SerializeField] private float shieldImpactCost = 10f;
//	[SerializeField] private float enemyShieldImpactCost = 30f;
//	[SerializeField] private float blockCost = 1f;
//	[SerializeField] private float sprintCost = 2f;

	private bool regenEnabled = true;
	private bool staminaLoosingState = false;
	
	[SerializeField, Range(0f, 100f)]
	private float _currentStamina;
	private float currentStamina {
		get => Mathf.Clamp(_currentStamina, 0, maxStamina);
		set {
			value = Mathf.Clamp(value, 0, maxStamina);
			
			if (Mathf.Approximately(value, _currentStamina))
				return;

			_currentStamina = value;
			OnStaminaChanged(_currentStamina);
		}
	}

	public bool AllowAttack() {
		return true;
	}
	
	public bool AllowBlock() {
//		if (currentStamina < dodgeCost) {
//			OnNotEnoughStaminaForAction();
//			return false;
//		}

//		LoseStamina(Time.deltaTime * 0.01f);
		return true;
	}

	public bool AllowDodge() {

		if (currentStamina < dodgeCost) {
			OnNotEnoughStaminaForAction();
			return false;
		}

		LoseStamina(dodgeCost);
		return true;
	}

	private void LoseStamina(float lostStamina) {
		currentStamina -= lostStamina;
		
		regenEnabled = false;
		staminaLoosingState = false;
		
		CancelInvoke(nameof(RegainStaminaRegen));
		Invoke(nameof(RegainStaminaRegen), regainStaminaRegenAfterLossTimeout);
	}

	private void RegainStaminaRegen() {
		regenEnabled = true;
	}

	private void Update() {
		if (regenEnabled) {
			RegenStamina();
		}
	}

	private void RegenStamina() {
		currentStamina += Time.deltaTime * staminaRegenPerSec;
	}

	private void Awake() {
		currentStamina = maxStamina;
	}
}
