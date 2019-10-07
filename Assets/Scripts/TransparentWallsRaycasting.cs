using System.Collections.Generic;
using UnityEngine;

public class TransparentWallsRaycasting : MonoBehaviour {

	private List<MeshRenderer> transparentRenderers = new List<MeshRenderer>();

	private void Update() {
		// ResetRenderers();
		// if (GM.instance?.mainCamera == null || GM.player == null) return;
		
		// var fromPos = GM.instance.mainCamera.transform.position;
		// var targetPos = GM.player.transform.position;
		// var targetDir = targetPos - fromPos;

		// Debug.DrawRay(fromPos, targetDir, Color.white, Time.deltaTime, true);

		// if (!Physics.Raycast(
		// 	fromPos, 
		// 	targetDir, 
		// 	out var hitInfo)
		// )
		// 	return;

		// if (!hitInfo.transform.CompareTag("Enviro"))
		// 	return;

		// var newRenderer = hitInfo.transform.gameObject.GetComponent<MeshRenderer>();
		// transparentRenderers.Add(newRenderer);

		// var asd = new MaterialPropertyBlock();
		// asd.SetFloat("_transparency", 1f);
		// newRenderer.SetPropertyBlock(asd);
		// Debug.Log("found gO: ", hitInfo.transform.gameObject);
	}

	private void ResetRenderers() {
		foreach (var rend in transparentRenderers) {
			var asd = new MaterialPropertyBlock();
			asd.SetFloat("_transparency", 0f);
			rend.SetPropertyBlock(asd);
		}

		transparentRenderers.Clear();
	}
	
}
