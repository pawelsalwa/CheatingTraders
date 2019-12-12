using UnityEngine;
using System;

public class StaminaComponent : MonoBehaviour {

	public event Action<float, float> OnStaminaChanged = (staminaChanged, xd) => { };
	public event Action OnNotEnoughStaminaForAction = () => { };
	
	float dodgeCost = 40f;
	[SerializeField] private float maxStamina = 100f;
	[SerializeField] private float staminaRegenPerSec = 20f;
	[SerializeField] private float staminaUILoosingPerSec = 40f;
	[SerializeField, Range(0f, 5f)] private float regainStaminaAfterLossTimeout = 2f;
	[SerializeField, Range(0f, 5f)] private float staminaLoosingUIShadeTimeout = 2f; // shade on ui following stamina amount for ux effect
	[SerializeField] private float currentTimeoutAfterStaminaLoss = 0f;

	private bool regenEnabled = true;
	private bool staminaLoosingState = false;
	private bool staminaUIShadeLoosingState = false;
	
	[SerializeField, Range(0f, 100f)]
	private float _currentStamina;
	private float currentStamina {
		get => Mathf.Clamp(_currentStamina, 0, maxStamina); // to be sure...
		set {
			_currentStamina = Mathf.Clamp(value, 0, maxStamina);
			OnStaminaChanged(_currentStamina, _staminaOnUI);
			DevTools.Print("stamina: " + value, DevTools.DebugColor.magenta);
		}
	}
	
	[SerializeField, Range(0f, 100f)]
	private float _staminaOnUI;
	private float staminaOnUI {
		get => Mathf.Clamp(_staminaOnUI, currentStamina, maxStamina); // to be sure...
		set {
			_currentStamina = Mathf.Clamp(value, currentStamina, maxStamina);
			OnStaminaChanged(_currentStamina, _staminaOnUI);
			
			DevTools.Print("ui stamina: " + value, DevTools.DebugColor.green);
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
		
		CancelInvoke(nameof(LooseStaminaOnUI));
		Invoke(nameof(LooseStaminaOnUI), staminaLoosingUIShadeTimeout);
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

		if (staminaLoosingState) {
			LooseUIStamina();
		}
			
	}

	private void RegenStamina() {
		currentStamina += Time.deltaTime * staminaRegenPerSec;
		staminaOnUI = currentStamina;
	}
	
	private void LooseUIStamina() {
		staminaOnUI -= Time.deltaTime * staminaUILoosingPerSec;
	}

	private void Awake() {
		currentStamina = maxStamina;
	}
}
