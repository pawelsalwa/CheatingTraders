using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : MonoBehaviour {

	private Animator _animator;

	private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;


	private void Update() {
		animator.SetBool("attacking", Input.GetKey(KeyCode.Mouse0));
	}
}