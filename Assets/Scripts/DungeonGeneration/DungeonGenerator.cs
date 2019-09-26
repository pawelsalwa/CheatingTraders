using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(DungeonMap))]
public class DungeonGenerator : MonoBehaviour {

	public int roomMinSize, roomMaxSize, maxRoomNum = 6;
	public int minTileX, maxTileX, minTileZ, maxTileZ;

	public int maxRoomDistanceGeneration = 10;

	private DungeonMap _map;
	private DungeonMap map => _map == null ? _map = GetComponent<DungeonMap>() : _map;

	private List<RoomSetup> roomsList = new List<RoomSetup>();

	public void Generate() {
		Deregenerate();
//		GenerateRandomRooms();
//		GenerateCorridors();
		for (int i = 0; i < maxRoomNum; i++) {
			GenerateRandomRoomAndConnectIt();
		}

		map.CreateAllWalls();
	}

	public Vector3 GetPlayerStartingPosition() {
		var xd = map.GetDungeonTile(roomsList[0].GetMiddlePoint().x, roomsList[0].GetMiddlePoint().y);
		return xd.transform.position + Vector3.up;
	}

	public void GenerateRandomRoomAndConnectIt() {
		while (true) {
			if (EditorUtility.DisplayCancelableProgressBar("Dungeon Generator", $"generating room nr {roomsList.Count}...", 0f)) {
				Debug.Log("Dungeon Generator canceled by the user");
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
			GenerateRoom(newRoom.minx, newRoom.minz, newRoom.widthx, newRoom.heightz, roomsList.Count - 1);
			if (roomsList.Count > 0)
				GenerateCorridor(newRoom, newRoom.GetClosestRoom(roomsList));
			break;
		}

		EditorUtility.ClearProgressBar();
		map.ReregenerateWalls();
	}

	private void GenerateRandomRooms() {
		map.ClearMap();
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
			GenerateRoom(newRoom.minx, newRoom.minz, newRoom.widthx, newRoom.heightz, roomNum);
			roomNum++;
		}

		EditorUtility.ClearProgressBar();
	}

	private void GenerateRoom(int xmin, int zmin, int xwidth, int zheight, int roomIndex) {
		for (int x = xmin; x < xmin + xwidth; x++)
			for (int z = zmin; z < zmin + zheight; z++)
				GenerateTile(x, z, DungeonTile.TileType.Room, roomIndex);
	}

	private void GenerateTile(int x, int z, DungeonTile.TileType tileType, int roomIndex) {
		map.AddTile(x, z, tileType, roomIndex);
	}

	private void GenerateCorridors() {
		foreach (var room in roomsList) {
			if (EditorUtility.DisplayCancelableProgressBar("Dungeon Generator", "generating corridors...", 0f)) {
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
				GenerateTile(currentTile.x, i, DungeonTile.TileType.Corridor, 0);
				currentTile.y = i;
			}
		}

		void GenerateHorizontalPart() {
			for (int i = xd1.x; i != xd2.x; i += xd1.x < xd2.x ? 1 : -1) {
				GenerateTile(i, currentTile.y, DungeonTile.TileType.Corridor, 0);
				currentTile.x = i;
			}
		}
	}

	/// <param name="random"></param>
	/// <param name="mean"></param>
	/// <param name="stddev"></param>
	/// <returns></returns>
	public static double GetRandomGaussianNumber(System.Random random, double mean = 0, double stddev = 1) {
		double x1 = 1 - random.NextDouble();
		double x2 = 1 - random.NextDouble();

		double y1 = Math.Sqrt(-2.0f * Math.Log((float) x1)) * Math.Cos(2.0f * Math.PI * x2);
		return y1 * stddev + mean;
	}

	public void Deregenerate() {
		map.ClearMap();
		roomsList.Clear();
	}

	[InitializeOnLoadMethod]
	private static void xd() {
		EditorUtility.ClearProgressBar();
	}
}