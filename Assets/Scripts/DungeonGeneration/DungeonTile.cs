using UnityEngine;

public class DungeonTile : MonoBehaviour {

	public enum TileType { Room, Corridor }

	public TileType tileType;

	public int x, z;
}