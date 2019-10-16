using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TransparentWallsRaycasting : MonoBehaviour {

	public List<MeshRenderer> transparentRenderers = new List<MeshRenderer>();

	/// <summary> from pos is behind camera to make sure raycast from it hits all needed colliders </summary>
	private Vector3 fromPos => GM.instance.mainCamera.transform.position - GM.instance.mainCamera.transform.forward.normalized;

	private Vector3 targetPos => GM.player.transform.position + Vector3.up;
	private Vector3 targetDirCenter => targetPos - fromPos;
	private Vector3 targetDirLeft => GM.player.transform.position - GM.player.transform.right * 2f - fromPos + Vector3.up;
	private Vector3 targetDirRight => GM.player.transform.position + GM.player.transform.right * 2f - fromPos + Vector3.up;

	private Vector3 targetDirLeft2 => GM.player.transform.position - GM.player.transform.right * 4f - fromPos + Vector3.up;
	private Vector3 targetDirRight2 => GM.player.transform.position + GM.player.transform.right * 4f - fromPos + Vector3.up;

	private float distanceToPlayer => Vector3.Magnitude(fromPos - targetPos);

	private void Update() {
		if (GM.instance?.mainCamera == null || GM.player == null) return;

		ResetRenderers();

		Debug.DrawRay(fromPos, targetDirCenter, Color.cyan, Time.deltaTime, true);
		Debug.DrawRay(fromPos, targetDirLeft, Color.cyan, Time.deltaTime, true);
		Debug.DrawRay(fromPos, targetDirRight, Color.cyan, Time.deltaTime, true);
		Debug.DrawRay(fromPos, targetDirLeft2, Color.cyan, Time.deltaTime, true);
		Debug.DrawRay(fromPos, targetDirRight2, Color.cyan, Time.deltaTime, true);

		if (!IsCameraBehindWall()) { return; }

		Physics.Raycast(fromPos, targetDirCenter,  out var hitInfos);
		Physics.Raycast(fromPos, targetDirLeft,  out var hitInfos1);
		Physics.Raycast(fromPos, targetDirRight,  out var hitInfos2);
		Physics.Raycast(fromPos, targetDirLeft2,  out var hitInfos3);
		Physics.Raycast(fromPos, targetDirRight2,  out var hitInfos4);

//		if (hitInfos.Length == 0) return;

		SetTransparentOnMaterials(hitInfos);
		SetTransparentOnMaterials(hitInfos1);
		SetTransparentOnMaterials(hitInfos2);
		SetTransparentOnMaterials(hitInfos3);
		SetTransparentOnMaterials(hitInfos4);
	}

	private bool IsCameraBehindWall() {
		return Physics.Raycast(fromPos, targetDirCenter, out var hitInfo, distanceToPlayer) && hitInfo.transform.CompareTag("Enviro");
	}

	private void SetTransparentOnMaterials(RaycastHit hitInfo) {

//		foreach (var hitInfo in hitInfos) {
			if (!hitInfo.transform.CompareTag("Enviro"))
				return;

			var newRenderer = hitInfo.transform.gameObject.GetComponent<MeshRenderer>();
			if (transparentRenderers.Contains(newRenderer))
				return;

			transparentRenderers.Add(newRenderer);
			var mpBlock = new MaterialPropertyBlock();
			mpBlock.SetInt("_isWorking", 1);
			newRenderer.SetPropertyBlock(mpBlock);
//		}
	}

	private void ResetRenderers() {
		foreach (var rend in transparentRenderers) {
			try {
				var mpBlock = new MaterialPropertyBlock();
				mpBlock.SetInt("_isWorking", 0);
				rend.SetPropertyBlock(mpBlock);
			} catch (Exception) { }
		}

		transparentRenderers.Clear();
	}

}