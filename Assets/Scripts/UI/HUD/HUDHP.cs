using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HUDHP : MonoBehaviour {

	[SerializeField] private Image hpBarImage;
	[SerializeField] private Image bgcImage;
	[SerializeField] private HUDStaminaShade hpShade;

	private Color barNormalColor;
	private Color bgcNormalColor;
	
	[SerializeField] private Color barHighlightedColor;
	[SerializeField] private Color bgcHighlightedColor;
	[SerializeField] private float highlightTimeoutSecs = 0.5f;
	
	private float maxHP => GM.projectConstants.unit.hp.maxHP;

	private void Awake() {
		GM.OnPlayerSpawned += SubscribePlayerEvents;
		barNormalColor = hpBarImage.color;
		bgcNormalColor = bgcImage.color;
	}

	private void SubscribePlayerEvents(BasicUnit player) {
		player.OnHPChanged += SetHPBar;
		player.OnDeath += () => UnsubscribePlayerEvents(player);
	}

	private void UnsubscribePlayerEvents(BasicUnit player) {
		player.OnHPChanged -= SetHPBar;
	}

	private void SetHPBar(float hp) {
		hpBarImage.fillAmount = hp / maxHP;
		hpShade.currentStamina = hp;
	}

	private void Highlighthp() {
		hpBarImage.color = barHighlightedColor;
		bgcImage.color = bgcHighlightedColor;
		
		CancelInvoke(nameof(StopHighlightingHP));
		Invoke(nameof(StopHighlightingHP), highlightTimeoutSecs);
	}

	private void StopHighlightingHP() {
		CancelInvoke(nameof(StopHighlightingHP));
		hpBarImage.color = barNormalColor;
		bgcImage.color = bgcNormalColor;
	}

}