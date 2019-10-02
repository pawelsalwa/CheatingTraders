using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgingComponent : MonoBehaviour {
    
    
    private Animator _animator;
    protected Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;
    
    public void Dodge() {
        animator.SetTrigger("dodgeTrigger");
    }

    public void MoveW() { animator.SetFloat("dodgeFrontFactor", 1f); animator.SetFloat("dodgeRightFactor", 0f); }

	public void MoveS() { animator.SetFloat("dodgeFrontFactor", -1f); animator.SetFloat("dodgeRightFactor", 0f); }

	public void MoveA() { animator.SetFloat("dodgeFrontFactor", 0f); animator.SetFloat("dodgeRightFactor", -1f); }

	public void MoveD() { animator.SetFloat("dodgeFrontFactor", 0f); animator.SetFloat("dodgeRightFactor", 1f); }

	public void MoveWA() { animator.SetFloat("dodgeFrontFactor", 1f); animator.SetFloat("dodgeRightFactor", -1f); }

	public void MoveWD() { animator.SetFloat("dodgeFrontFactor", 1f); animator.SetFloat("dodgeRightFactor", 1f); }

	public void MoveSA() { animator.SetFloat("dodgeFrontFactor", -1f); animator.SetFloat("dodgeRightFactor", -1f); }

	public void MoveSD() { animator.SetFloat("dodgeFrontFactor", -1f); animator.SetFloat("dodgeRightFactor", 1f); }

	public void DontMove()  { animator.SetFloat("dodgeFrontFactor", 0f); animator.SetFloat("dodgeRightFactor", 0f); }
}
