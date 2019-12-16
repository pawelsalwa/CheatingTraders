using System;
using UnityEngine;

public class ProjectConstants : MonoBehaviour {
	
	public int playerLayer = 8;
	public int botLayer = 9;

	public UnitConstants unit;

	public bool combatDebugs = false;

	public void ResetConstants() {
		float maxPlayerStamina = 100f;
		float playerStaminaRegenPerSec = 20f;

		float maxBotStamina = 100f;
		float botStaminaRegenPerSec = 20f;
	}

	[Serializable]
	public class UnitConstants {


		public int playerHP = 100;
		public int botHP = 100;
		public StaminaConstants stamina;

		[Serializable]
		public class StaminaConstants {
			public float maxStamina = 100f;
			public float staminaRegenPerSec = 70f;
			
			public float dodgeStaminaCost = 90f;
			public float superAttackStaminaCost = 100f;
			
			[Range(0f, 5f)] public float regainStaminaRegenAfterLossTimeout = 2f;
		}

		[Serializable]
		public class MovementConstants {

			public float dodge;

		}

	}
	
	
}