using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FaceDirection { horizontal, vertical }

public enum Direction { UP, DOWN, LEFT, RIGHT }

public class DungeonGenerator : MonoBehaviour {

	public DungeonTile corridorPrefab;

	private Dictionary<int, int> map = new Dictionary<int, int>();

	[ContextMenu("Generate")]
	public void Generate() {
		GenerateCorridorTile(0, 0);
	}

	private void GenerateCorridorTile(int x, int y) {
		if (DoesMapContainTile(x, y)) {
			Debug.LogError("Trying to create tile on existing tile.");
			return;
		}

		map[x] = y;
		var go = Instantiate(corridorPrefab);
		go.transform.position = new Vector3(0, 0, 0);
	}

	private bool DoesMapContainTile(int x, int y) {
		return map.ContainsKey(x) && map[x] == y;
	}

	private void GenerateStraightCorridor(int length, Direction dir) {
		for (int i = 0; i < length; i++) {
//			GenerateCorridorTile()
		}
	}

}