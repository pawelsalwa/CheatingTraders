using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum FaceDirection { horizontal, vertical }

public enum Direction { UP, DOWN, LEFT, RIGHT }

public class DungeonGenerator : MonoBehaviour {

	public DungeonCorridor corridorPrefab;
	public GameObject tilePrefab;

	public float xTileSize, zTileSize;
	public int xToGen, zToGen;
	public int roomMinSize, roomMaxSize, maxRoomNum = 6;
	public int minTileX, maxTileX, minTileZ, maxTileZ;

	private DungeonMap map = new DungeonMap();

	private List<RoomSetup> roomsList = new List<RoomSetup>();

	public Direction direction;

	[ContextMenu("Generate")]
	public void Generate() {
//		GenerateCorridorTile(xToGen, zToGen);
//		GenerateStraightCorridor(10 ,xToGen, zToGen, direction );
//		GenerateRandomCorridors();
		Deregenerate();
		GenerateRandomRooms();
		GenerateCorridors();
	}

	private void GenerateRandomRooms() {
		map = new DungeonMap();
		var roomNum = 0;
		roomsList.Clear();

		while (roomNum != maxRoomNum) {
			if (EditorUtility.DisplayCancelableProgressBar("Dungeon Generator", "generating...", 0f)) {
				Debug.Log("Dungeon Generator canceled by the user");
				Deregenerate();
				break;
			}

			var newRoom = new RoomSetup {
				minx = Random.Range(minTileX, maxTileX),
				minz = Random.Range(minTileZ, maxTileZ),
				widthx = Random.Range(roomMinSize, roomMaxSize),
				heightz = Random.Range(roomMinSize, roomMaxSize)
			};

			if (roomsList.Any(x => x.Intersects(newRoom)))
				continue;

			roomsList.Add(newRoom);
			GenerateRoom(newRoom.minx, newRoom.minz, newRoom.widthx, newRoom.heightz);
			roomNum++;
		}

		EditorUtility.ClearProgressBar();
	}

	private void GenerateRoom(int xmin, int zmin, int xwidth, int zheight) {
		for (int x = xmin; x < xmin + xwidth; x++)
			for (int z = zmin; z < zmin + zheight; z++)
				GenerateTile(x, z, DungeonMap.SingleDungeonTile.TileType.Room);
	}

	private void GenerateTile(int x, int z, DungeonMap.SingleDungeonTile.TileType tileType) {
		if (map.DoesMapContainTile(x, z)) {
			Debug.LogWarning("Trying to create tile on existing tile.");
			return;
		}

		map.AddTile(x, z, tileType);

		var go = Instantiate(tilePrefab, transform);
		go.transform.position = new Vector3(xTileSize * x, 0, zTileSize * z);
	}

	private void GenerateCorridors() {
		foreach (var room in roomsList) {
			if (EditorUtility.DisplayCancelableProgressBar("Dungeon Generator", "generating...", 0f)) {
				Debug.Log("Dungeon Generator canceled by the user");
				Deregenerate();
				break;
			}

			GenerateCorridor(room, room.GetClosestRoom(roomsList));
		}

		EditorUtility.ClearProgressBar();
	}

	private void GenerateCorridor(RoomSetup roomFrom, RoomSetup roomTo) {
		var xd1 = roomFrom.GetMiddlePoint();
		var xd2 = roomTo.GetMiddlePoint();
		Vector2Int currentTile = new Vector2Int(xd1.x, xd1.y);

		GenerateVerticalPart();
		GenerateHorizontalPart();

		void GenerateVerticalPart() {
			for (int i = xd1.y; i != xd2.y; i += xd1.y < xd2.y ? 1 : -1) {
				GenerateTile(currentTile.x, i, DungeonMap.SingleDungeonTile.TileType.Corridor);
				currentTile.y = i;
			}
		}

		void GenerateHorizontalPart() {
			for (int i = xd1.x; i != xd2.x; i += xd1.x < xd2.x ? 1 : -1) {
				GenerateTile(i, currentTile.y, DungeonMap.SingleDungeonTile.TileType.Corridor);
				currentTile.x = i;
			}
		}
	}

	public void Deregenerate() {
		for (int i = transform.childCount - 1; i >= 0; --i)
#if UNITY_EDITOR
			if (EditorApplication.isPlaying)
				Destroy(transform.GetChild(i).gameObject);
			else
				DestroyImmediate(transform.GetChild(i).gameObject);
#else
			Destroy(transform.GetChild(i).gameObject);
#endif
		map = new DungeonMap();
	}

	public class RoomSetup {
		public bool isConnected = false;

		public int minx, minz, widthx, heightz;
		public int maxx => minx + widthx;
		public int maxz => minz + heightz;

		public bool Intersects(RoomSetup other) {
			return minx <= other.maxx &&
			       maxx >= other.minx &&
			       minz <= other.maxz &&
			       other.maxz >= other.minz;
		}

		public int GetDistance(RoomSetup other) {
			return other == this ? int.MaxValue : Mathf.Abs(minx - other.minx) + Mathf.Abs(minz - other.minz);
		}

		public RoomSetup GetClosestRoom(List<RoomSetup> rooms) {
			return rooms.OrderBy(x => x.GetDistance(this)).ToList()[0];
		}

		public Vector2Int GetMiddlePoint() {
			return new Vector2Int(
				Mathf.FloorToInt((minx + maxx) / 2),
				Mathf.FloorToInt((minz + maxz) / 2));
		}
	}
}