using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TransparencySetter : MonoBehaviour {

    public Material material;

    void Update () {
        if (GM.player == null) return;

        var pos = GM.player.transform.position;
        Vector4 playerPos4 = new Vector4 (pos.x, pos.y, pos.z, 0f);

        var dir = GM.player.transform.forward;
        Vector4 playerDir4 = new Vector4 (dir.x, dir.y, dir.z, 0f);

//        Debug.Log("playerPos: " + pos + "  - playerDir: " + dir);
//        Debug.Log("dot product: " + Vector3.Dot(dir, Vector3.Normalize(pos - objecta.position)));

        material.SetVector ("_playerPos", playerPos4);
        material.SetVector ("_playerDir", playerDir4);
    }

}