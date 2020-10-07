using UnityEngine;
public static class Define{
    public const int ROOM_RAD = 4;
    public const int MAP_SIZE = 1024;
    public const int MAP_SIZE_SQUARE = MAP_SIZE * MAP_SIZE;
}

public struct Location{
	public Location(Vector3 p, Quaternion r){
		Pos = p;
		Rot = r;
	}
	public Vector3 			Pos;
	public Quaternion 		Rot;
}

public enum Direction{
	Front 	= 1,
	Left 	= 2,
	Right 	= 3
}

public enum Orientation{
	North	= 0,
	East 	= 1,
	South 	= 2,
	West 	= 3
}
