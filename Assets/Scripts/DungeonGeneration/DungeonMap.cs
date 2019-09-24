using System;
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
		dgTiles.Add(newOne);
	}

	private DungeonTile GetDungeonTile(int x, int z) {
		return dgTiles.FirstOrDefault(a => a.x == x && a.z == z);
	}

//	private void CreateWalls(DungeonMap.SingleDungeonTile tile) {
//		if (map.DoesMapContainTile(tile.x + 1, tile.z)) {
//			GameObject newOne = Instantiate(wallPrefab, tile.transform);
//			newOne.transform.Translate(Vector3.right * xTileSize / 2);
//		}
//	}

	public void ClearMap() {
		foreach (var tile in dgTiles) {
#if UNITY_EDITOR
			if (EditorApplication.isPlaying)
				Destroy(tile.gameObject);
			else
				DestroyImmediate(tile.gameObject);
#else
				Destroy(xd.gameObject);
#endif
		}

		dgTiles.Clear();
		dgTiles = new List<DungeonTile>();
	}
}