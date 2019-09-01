using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotationComponent : MonoBehaviour
{

    public float _rotationSmoothFactor = 0.4f;

    private Transform cameraRot => GM.instance.camera.gameObject.transform;
    
    private void Update() {
        var targetRot = Quaternion.LookRotation(Vector3.Normalize(new Vector3(cameraRot.forward.x, 0f, cameraRot.forward.z)));
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, _rotationSmoothFactor);
    }
    
}