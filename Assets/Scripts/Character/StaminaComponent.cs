using UnityEngine;
using System;

public class StaminaComponent : MonoBehaviour {

	public event Action<float> OnStaminaChanged = (staminaChanged) => { };
	public event Action OnNotEnoughStaminaForAction = () => { };

	private float maxStamina => GM.projectConstants.unit.stamina.maxStamina;
	private float staminaRegenPerSec => GM.projectConstants.unit.stamina.staminaRegenPerSec;
	private float regainStaminaRegenAfterLossTimeout => GM.projectConstants.unit.stamina.regainStaminaRegenAfterLossTimeout;
	private float dodgeCost => GM.projectConstants.unit.stamina.dodgeStaminaCost;
	private float blockCost => GM.projectConstants.unit.stamina.blockStaminaCost;

	private bool regenEnabled = true;
	
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
		if (currentStamina < blockCost) {
			OnNotEnoughStaminaForAction();
			return false;
		}

		LoseStamina(blockCost);
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
