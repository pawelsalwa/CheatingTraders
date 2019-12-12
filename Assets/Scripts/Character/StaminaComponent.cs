using UnityEngine;
using System;

public class StaminaComponent : MonoBehaviour {

	public event Action<float> OnStaminaChanged = staminaChanged => { };
	public event Action OnNotEnoughStaminaForAction = () => { };
	
	float dodgeCost = 40f;
	[SerializeField] private float maxStamina = 100f;
	[SerializeField] private float staminaRegenPerSec = 20f;

	[SerializeField, Range(0f, 100f)] private float _currentStamina;
	private float currentStamina {
		get => Mathf.Clamp(_currentStamina, 0, maxStamina); // to be sure...
		set {
			_currentStamina = Mathf.Clamp(value, 0, maxStamina);
			OnStaminaChanged(_currentStamina);
		}
	}

	public bool AllowDodge() {

		if (currentStamina < dodgeCost) {
			OnNotEnoughStaminaForAction();
			return false;
		}
		
		currentStamina -= dodgeCost;
		return true;
	}

	private void Update() {
		currentStamina += Time.deltaTime * staminaRegenPerSec;
	}
}
