using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : MonoBehaviour {

	public ControlledBy controlledBy = ControlledBy.Player;

	private Animator _animator;

	private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

	private void Update() {
		if (controlledBy == ControlledBy.AI) return;
		animator.SetBool("attacking", Input.GetKey(KeyCode.Mouse0));
	}

	private void SwordHitEvent() { // idzcie do animacji i tam szukajcie SwordHitEvent iks de
		Debug.Log("Animation hit sth");
	}
}