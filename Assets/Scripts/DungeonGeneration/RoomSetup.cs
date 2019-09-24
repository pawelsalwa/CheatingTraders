using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
