using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary> sets stamina bar assuming the max is 100 (might need refactor if stamina max amount varies) </summary>
public class HUDStamina : MonoBehaviour {

	[SerializeField] private Image staminaBarImage;
	[SerializeField] private Image bgcImage;
	[SerializeField] private HUDStaminaShade staminaShade;

	private Color barNormalColor;
	private Color bgcNormalColor;
	
	[SerializeField] private Color barHighlightedColor;
	[SerializeField] private Color bgcHighlightedColor;
	[SerializeField] private float highlightTimeoutSecs = 0.5f;
	
	private float maxStamina => GM.projectConstants.unit.stamina.maxStamina;

	private void Awake() {
		GM.OnPlayerSpawned += SubscribePlayerEvents;
		barNormalColor = staminaBarImage.color;
		bgcNormalColor = bgcImage.color;
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
		staminaBarImage.fillAmount = stamina / maxStamina;
		staminaShade.currentStamina = stamina;
	}

	private void HighlightStamina() {
		staminaBarImage.color = barHighlightedColor;
		bgcImage.color = bgcHighlightedColor;
		
		CancelInvoke("StopHighlightingStamina");
		Invoke("StopHighlightingStamina", highlightTimeoutSecs);
	}

	private void StopHighlightingStamina() {
		CancelInvoke("StopHighlightingStamina");
		staminaBarImage.color = barNormalColor;
		bgcImage.color = bgcNormalColor;
	}

}