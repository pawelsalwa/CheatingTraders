using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMainCam : MonoBehaviour {
    void Update() {
        transform.LookAt(GM.instance.mainCamera.transform);
    }
}
