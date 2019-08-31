using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> Acts as layer of abstraction between user input and referenced components </summary>
public class UserInputHandler : MonoBehaviour
{
    public bool handleMovement = true;
    public MovementComponent movement;
    public bool handleAttack = true;
    public AttackComponent attack;

    private Vector3 moveDir;

    private void Awake() {
        GM.instance.gameMenu.OnOpened += () => enabled = false;
        GM.instance.gameMenu.OnClosed += () => enabled = true;
    }

    private void Update() {
        if (handleMovement)
            HandleMovement();
        if (handleAttack)
            HandleAttack();
    }

    private void HandleAttack() {
        if (Input.GetKey(KeyCode.Mouse1)) {
            attack.StopAttacking();
            attack.ContinueToBlock();
            return;
        } else 
            attack.StopBlocking();
        
        if (Input.GetKey(KeyCode.Mouse0))
            attack.ContinueToAttack();
        else 
            attack.StopAttacking();
        
    }

    private void HandleMovement() {
//        moveDir = Vector3.zero;
//
        if (Input.GetKey(KeyCode.W))
            movement.WalkForward();
        else 
            movement.NoMove();
//
//        if (Input.GetKey(KeyCode.S))
//            moveDir += cameraRot.forward * -1;

        if (Input.GetKey(KeyCode.A))
        {
//            moveDir += cameraRot.right * -1;
            movement.StrafeLeft();
        }

//        if (Input.GetKey(KeyCode.D))
//            moveDir += cameraRot.right;

//        movement.Move(moveDir);
    }
}
