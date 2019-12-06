using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TransparencySetter : MonoBehaviour {

	public Renderer wallsRenderer;
	public Material wallsSharedMaterial => wallsRenderer.sharedMaterial;

	void Update() {
		if (GM.player == null || wallsRenderer == null) return;

		var pos = GM.player.transform.position;
		Vector4 playerPos4 = new Vector4(pos.x, pos.y, pos.z, 0f);

		var dir = GM.player.transform.forward;
		Vector4 playerDir4 = new Vector4(dir.x, dir.y, dir.z, 0f);

		wallsSharedMaterial.SetVector("_playerPos", playerPos4);
		wallsSharedMaterial.SetVector("_playerDir", playerDir4);
	}

}