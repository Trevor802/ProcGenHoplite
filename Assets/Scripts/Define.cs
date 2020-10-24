using UnityEngine;
public static class DEF{
    public const int ROOM_RAD = 4;
    public const int MAP_SIZE = 1024;
    public const int MAP_SIZE_SQUARE = MAP_SIZE * MAP_SIZE;
    public const string ROOM_PATH = "Assets/Prefabs/Resources/Rooms";
	public const string NULL_ROOM = "00000000";
	public static readonly int[] NULL_ID = {-1, -1, -1, -1, -1, -1, -1, -1};
	public const float LIMBO_CHECK = 0.5f;
	public const int ATK_DIST = 1;
	public const int TRW_DIST_MIN = 2;
	public const int PSH_WAL_DMG = 1;
	public const int PSH_MANA = 3;
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
    Center    = 0,
	Left 	= 1,
	Right 	= 2,
	Front 	= 3
}

public enum Orientation{
	North	= 0,
	East 	= 1,
	South 	= 2,
	West 	= 3
}
public enum TileType : byte{
	Wall    = 0,
	Door    = 1,
	Floor   = 2,
	Lava    = 3,
}