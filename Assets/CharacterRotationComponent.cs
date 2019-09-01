using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotationComponent : MonoBehaviour {

    public float _rotationSmoothFactor = 0.4f;
    public float _rotationAnimationSmoothFactor = 0.01f;

    public Transform head;
    public float rotationSpeed;
    public string rotationAnimKey = "rotationFactor";

    public float maxRotSpeed = 5f;
    public float minRotSpeed = -5f;
    
    public float multiplier; // wspolczynniki rownania jak przekształacamy rotacje z [-5,5] do [0,1] -> ax + b = y => a=0.1, b=0.5
    public float addon;
	
    private Animator _animator;
    private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

    private Transform cameraRot => GM.instance.camera.gameObject.transform;

    private void Awake() {
        animator.SetFloat(rotationAnimKey, 0.5f);
    }
    
    private void Update() {
//        Vector3 prevRot = transform.rotation.eulerAngles;
        
        var targetRot = Quaternion.LookRotation(Vector3.Normalize(new Vector3(cameraRot.forward.x, 0f, cameraRot.forward.z)));
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, _rotationSmoothFactor);
        
//        Vector3 newRot = transform.rotation.eulerAngles;
//        rotationSpeed = prevRot.y - newRot.y;

        rotationSpeed = Input.GetAxis("Horizontal");
//        Debug.Log(rotationSpeed);

//        animator.SetFloat(rotationAnimKey, rotationSpeed * multiplier, 0.1f, Time.deltaTime);
        animator.SetFloat(rotationAnimKey, rotationSpeed * multiplier + addon, _rotationAnimationSmoothFactor, Time.deltaTime);

//        head.transform.rotation = targetRot; jak ogarne fbxy to sie zrobi
    }
}