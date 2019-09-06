using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CharacterRotationComponent : NetworkBehaviour {

    public float _rotationSmoothFactor = 0.4f;
    public float _rotationAnimationSmoothFactor = 0.01f;

    public string rotationAnimKey = "rotationFactor";
    
    public float animBlendTreeRotationMultiplier; // wspolczynniki rownania jak przekształacamy rotacje z [-5,5] do [0,1] -> ax + b = y => a=0.1, b=0.5
    public float addon;

    public float rotationSpeed;
	
    private Animator _animator;
    protected Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

    private Transform cameraRot => GM.instance.camera.gameObject.transform;

    public void LookAt(Vector3 targetRot) {
        Vector3 lastRot = transform.rotation.eulerAngles;
        var targetRotQua = Quaternion.LookRotation(Vector3.Normalize(new Vector3(targetRot.x, 0f, targetRot.z)));
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotQua, _rotationSmoothFactor);
        Vector3 newRot = transform.rotation.eulerAngles;

        rotationSpeed = lastRot.y - newRot.y;
        AnimateRotation(rotationSpeed);
    }

    protected void AnimateRotation(float speed) {
        speed = Mathf.Clamp(speed, -4f, 4f);
        animator.SetFloat(rotationAnimKey, speed, _rotationAnimationSmoothFactor, Time.deltaTime);
    }

    protected virtual void OnRotateRequested() { }
    
    
}