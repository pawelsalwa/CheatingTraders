using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgingComponent : MonoBehaviour {

	public event Action<float, float> OnDodgeRequested = (vertical, horizontal) => { };

	public float dodgingCooldown = 1.3f;
	private bool dodgingEnabled = true;

	public void MoveW() { RequestDodge(1f, 0f); }

	public void MoveS() { RequestDodge(-1f, 0f); }

	public void MoveA() { RequestDodge(0f, -1f); }

	public void MoveD() { RequestDodge(0f, 1f); }

	public void MoveWA() { RequestDodge(1f, -1f); }

	public void MoveWD() { RequestDodge(1f, 1f); }

	public void MoveSA() { RequestDodge(-1f, -1f); }

	public void MoveSD() { RequestDodge(-1f, 1f); }

	public void DontMove() { RequestDodge(0f, 0f); }

	private void RequestDodge(float x, float y) {
		if (!dodgingEnabled) return;
		
		OnDodgeRequested(x, y);
//		dodgingEnabled = false;
//		CancelInvoke(nameof(EnableDodging));
//		Invoke(nameof(EnableDodging), dodgingCooldown);
	}

	private void EnableDodging() {
		dodgingEnabled = true;
	}

}