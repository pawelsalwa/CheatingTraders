using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> sets stamina bar assuming the max is 100 (might need refactor if stamina max amount varies) </summary>
public class GUIStamina : MonoBehaviour {

	[SerializeField] private Image staminaBarImage;

	private Color barNormalColor;
	[SerializeField] private Color barHighlightedColor;

	[SerializeField] private float highlightTimeoutSecs = 0.5f;

	private void Awake() {
		GM.OnPlayerSpawned += SubscribePlayerEvents;
		barNormalColor = staminaBarImage.color;
	}

	private void SubscribePlayerEvents(BasicUnit player) {
		player.OnStaminaChanged += SetStaminaBar;
		player.OnNotEnoughStaminaForAction += HighlightStamina;
		player.OnDeath += () => UnsubscribePlayerEvents(player);
	}

	private void UnsubscribePlayerEvents(BasicUnit player) {
		player.OnStaminaChanged -= SetStaminaBar;
		player.OnNotEnoughStaminaForAction -= HighlightStamina;
	}

	private void SetStaminaBar(float stamina) {
		staminaBarImage.fillAmount = stamina / 100f;
	}

	private void HighlightStamina() {
		staminaBarImage.color = barHighlightedColor;
		CancelInvoke("StopHighlightingStamina");
		Invoke("StopHighlightingStamina", highlightTimeoutSecs);
	}

	private void StopHighlightingStamina() {
		CancelInvoke("StopHighlightingStamina");
		staminaBarImage.color = barNormalColor;
	}

}