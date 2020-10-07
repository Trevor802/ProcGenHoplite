using UnityEngine;
public static class Util{
    public static string ToQuat(this ushort value, int length = 8){
        var sb = new System.Text.StringBuilder();
        while(length-- > 0){
            var n = value % 4;
            sb.Insert(0, n);
            value /= 4;
        }
        string result = sb.ToString();
        return result;
    }

    public static Location[] GetLocations(this Location point){
		var points = new Location[4];
		points[0].Pos = point.Pos + point.Rot * new Vector3(-1, 0, 1) * Define.ROOM_RAD;
		points[0].Rot = point.Rot;
		// var _0 = new Location{
		// 	Pos = point.Pos + point.Rot * new Vector3(-1, 0, 1), 
		// 	Rot = point.Rot
		// };
		points[1].Pos = point.Pos + point.Rot * new Vector3(-2, 0, 2) * Define.ROOM_RAD;
		points[1].Rot = point.Rot;
		points[2].Pos = point.Pos + point.Rot * new Vector3(-2, 0, -2) * Define.ROOM_RAD;
		points[2].Rot = point.Rot * Quaternion.Euler(0, -90, 0); 
		// var _1 = new Location{ 
		// 	Pos = point.Pos + point.Rot * new Vector3(3, 0, 1),
		// 	Rot = point.Rot * Quaternion.Euler(0, -90, 0)
		// };
		points[3].Pos = point.Pos + point.Rot * new Vector3(2, 0, 2) * Define.ROOM_RAD;
		points[3].Rot = point.Rot * Quaternion.Euler(0, 90, 0); 
		// var _2 = new Location{
		// 	Pos = point.Pos + point.Rot * new Vector3(-1, 0, -3),
		// 	Rot = point.Rot * Quaternion.Euler(0, 90, 0)
		// };
		// points[0] = _0;
		// points[1] = _1;
		// points[2] = _2;
		return points;
	}

    public static int ToHash(this Location location, bool withRot){
        var x = Mathf.RoundToInt(location.Pos.x);
        var z = Mathf.RoundToInt(location.Pos.z);
        var result = x * Define.MAP_SIZE + z;
        if (withRot){
            result += (int)location.Rot.ToOrientation() * Define.MAP_SIZE_SQUARE;
        }
        return result;
    }

    public static Orientation ToOrientation(this Quaternion rotation){
        if (Quaternion.Angle(rotation, Quaternion.identity) < 0.5f){
            return Orientation.North;
        }
        if (Quaternion.Angle(rotation, Quaternion.Euler(0, -90f, 0)) < 0.5f){
            return Orientation.West;
        }
        if (Quaternion.Angle(rotation, Quaternion.Euler(0, 90f, 0)) < 0.5f){
            return Orientation.East;
        }
        if (Quaternion.Angle(rotation, Quaternion.Euler(0, 180f, 0)) < 0.5f){
            return Orientation.South;
        }
        throw new System.InvalidOperationException("Invalid angle");
    }
}