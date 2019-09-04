using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotationComponent : MonoBehaviour {

    public float _rotationSmoothFactor = 0.4f;
    public float _rotationAnimationSmoothFactor = 0.01f;

    public float rotationSpeed;
    public string rotationAnimKey = "rotationFactor";
    
    public float animBlendTreeRotationMultiplier; // wspolczynniki rownania jak przekształacamy rotacje z [-5,5] do [0,1] -> ax + b = y => a=0.1, b=0.5
    public float addon;
	
    private Animator _animator;
    private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

    private Transform cameraRot => GM.instance.mainCamera.gameObject.transform;

    private void Awake() {
        animator.SetFloat(rotationAnimKey, 0.5f);
    }

    public void LookAt(Vector3 targetRot) {
        var targetRotQua = Quaternion.LookRotation(Vector3.Normalize(new Vector3(targetRot.x, 0f, targetRot.z)));
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotQua, _rotationSmoothFactor);
    }

    public void AnimateRotation(float rotationSpeed) {
        animator.SetFloat(rotationAnimKey, rotationSpeed * animBlendTreeRotationMultiplier + addon, _rotationAnimationSmoothFactor, Time.deltaTime);
    }
    
    private void FixedUpdate() {
    }
}