using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonMap {

	private List<SingleDungeonTile> dgTiles = new List<SingleDungeonTile>();

	public bool DoesMapContainTile(int x, int z) {
		return dgTiles.Any(a => a.x == x && a.z == z);
	}

	public void AddTile(int nx, int nz, SingleDungeonTile.TileType tileType) {
		dgTiles.Add(new SingleDungeonTile {x = nx, z = nz, tileType = tileType});
	}

	public SingleDungeonTile GetRandomTileIdx() {
		return dgTiles[Random.Range(0, dgTiles.Count)];
	}

	public class SingleDungeonTile {

		public enum TileType { Room, Corridor }

		public TileType tileType;

		public int x, z;
	}
}