using UnityEngine;
using System;

public class StaminaComponent : MonoBehaviour {

	public event Action<float> OnStaminaChanged = (staminaChanged) => { };
	public event Action OnNotEnoughStaminaForAction = () => { };
	
	float dodgeCost = 40f;
	[SerializeField] private float maxStamina = 100f;
	[SerializeField] private float staminaRegenPerSec = 30f;
	[SerializeField, Range(0f, 5f)] private float regainStaminaAfterLossTimeout = 2f;

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
		Invoke(nameof(RegainStaminaRegen), regainStaminaAfterLossTimeout);
	}

	private void RegainStaminaRegen() {
		regenEnabled = true;
	}

	private void LooseStaminaOnUI() {
		staminaLoosingState = true;
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
