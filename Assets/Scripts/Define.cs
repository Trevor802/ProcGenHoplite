﻿using UnityEngine;
public static class Define{
    public const int ROOM_RAD = 4;
    public const int MAP_SIZE = 1024;
    public const int MAP_SIZE_SQUARE = MAP_SIZE * MAP_SIZE;
    public const string ROOM_PATH = "Assets/Prefabs/Resources/Rooms";
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
