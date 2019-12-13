using UnityEngine;

public class ProjectConstants : MonoBehaviour {
	public int playerLayer = 8;
	public int botLayer = 9;

	public float maxPlayerStamina = 100f;
	public float playerStaminaRegenPerSec = 20f;

	public float maxBotStamina = 100f;
	public float botStaminaRegenPerSec = 20f;

	public void ResetConstants() {
		float maxPlayerStamina = 100f;
		float playerStaminaRegenPerSec = 20f;

		float maxBotStamina = 100f;
		float botStaminaRegenPerSec = 20f;
	}

}