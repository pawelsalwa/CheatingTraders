using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputHandler : MonoBehaviour
{
    public MovementComponent movement;
    public AttackComponent attack;

    private Transform cameraRot => GM.instance.camera.gameObject.transform;

    private Vector3 moveDir;

    private void Update() {
        HandleMovement();
        HandleAttack();
    }

    private void HandleAttack() {
        if (Input.GetKey(KeyCode.Mouse0))
            attack.ContinueToAttack();
        else 
            attack.StopAttacking();
    }

    private void HandleMovement() {
        moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            moveDir += cameraRot.forward;

        if (Input.GetKey(KeyCode.S))
            moveDir += cameraRot.forward * -1;

        if (Input.GetKey(KeyCode.A))
            moveDir += cameraRot.right * -1;

        if (Input.GetKey(KeyCode.D))
            moveDir += cameraRot.right;

        movement.Move(moveDir);
    }
}
