using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgingComponent : MonoBehaviour {

	public event Action<float, float> OnDodgeRequested = (vertical, horizontal) => { };

	public void MoveW() { OnDodgeRequested(1f, 0f); }

	public void MoveS() { OnDodgeRequested(-1f, 0f); }

	public void MoveA() { OnDodgeRequested(0f, -1f); }

	public void MoveD() { OnDodgeRequested(0f, 1f); }

	public void MoveWA() { OnDodgeRequested(1f, -1f); }

	public void MoveWD() { OnDodgeRequested(1f, 1f); }

	public void MoveSA() { OnDodgeRequested(-1f, -1f); }

	public void MoveSD() { OnDodgeRequested(-1f, 1f); }

	public void DontMove() { OnDodgeRequested(0f, 0f); }

}