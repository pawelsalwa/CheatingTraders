﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonMap : MonoBehaviour {

	public DungeonTile tilePrefab;
	public GameObject wallPrefab;
	public float xTileSize = 6, zTileSize = 6;

	[SerializeField] private List<DungeonTile> dgTiles = new List<DungeonTile>();
	private List<GameObject> wallList = new List<GameObject>();

	private bool DoesMapContainTile(int x, int z) {
		return dgTiles.Any(a => a.x == x && a.z == z);
	}

	public void AddTile(int nx, int nz, DungeonTile.TileType tileType) {
		if (DoesMapContainTile(nx, nz)) {
			Debug.LogWarning($"Trying to create tile on existing tile. Coordinates: ({nx}, {nz})", GetDungeonTile(nx, nz)?.gameObject);
			return;
		}

		DungeonTile newOne = Instantiate(tilePrefab, transform);
		newOne.x = nx;
		newOne.z = nz;
		newOne.transform.position = new Vector3(xTileSize * nx, 0, zTileSize * nz);
		newOne.tileType = tileType;
		dgTiles.Add(newOne);
	}

	public DungeonTile GetDungeonTile(int x, int z) {
		return dgTiles.FirstOrDefault(a => a.x == x && a.z == z);
	}

	public void CreateAllWalls() {
		foreach (var tile in dgTiles)
			CreateWalls(tile);
	}

	private void CreateWalls(DungeonTile tile) {
		if (!DoesMapContainTile(tile.x + 1, tile.z)) {
			GameObject newOne = Instantiate(wallPrefab, tile.transform);
			newOne.transform.Translate(Vector3.right * xTileSize / 2);
			newOne.transform.Rotate(new Vector3(0, 90f, 0));
			wallList.Add(newOne);
		}

		if (!DoesMapContainTile(tile.x - 1, tile.z)) {
			GameObject newOne = Instantiate(wallPrefab, tile.transform);
			newOne.transform.Translate(Vector3.left * xTileSize / 2);
			newOne.transform.Rotate(new Vector3(0, 90f, 0));
			wallList.Add(newOne);
		}

		if (!DoesMapContainTile(tile.x, tile.z + 1)) {
			GameObject newOne = Instantiate(wallPrefab, tile.transform);
			newOne.transform.Translate(Vector3.forward * zTileSize / 2);
			wallList.Add(newOne);
		}

		if (!DoesMapContainTile(tile.x, tile.z - 1)) {
			GameObject newOne = Instantiate(wallPrefab, tile.transform);
			newOne.transform.Translate(Vector3.back * zTileSize / 2);
			wallList.Add(newOne);
		}
	}

	public void ClearMap() {
		foreach (var tile in dgTiles) {
#if UNITY_EDITOR
			if (EditorApplication.isPlaying)
				Destroy(tile.gameObject);
			else
				DestroyImmediate(tile.gameObject);
#else
				Destroy(tile.gameObject);
#endif
		}

		dgTiles.Clear();
		wallList.Clear();
	}

	public void ReregenerateWalls() {
		foreach (var tile in wallList) {
#if UNITY_EDITOR
			if (EditorApplication.isPlaying)
				Destroy(tile);
			else
				DestroyImmediate(tile);
#else
				Destroy(tile);
#endif
		}

		wallList.Clear();
		CreateAllWalls();
	}
}