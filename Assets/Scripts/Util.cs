using System;
using UnityEngine;
public static class Util{
    public static string ToQuatStr(this ushort value, int length = 8){
        var sb = new System.Text.StringBuilder();
        while(length-- > 0){
            var n = value % 4;
            sb.Insert(0, n);
            value >>= 2;
        }
        string result = sb.ToString();
        return result;
    }
    public static string ToQuatStr(this int[] array, int length = 8){
        var sb = new System.Text.StringBuilder();
        while(length-- > 0){
            sb.Append(array[length]);
        }
        string result = sb.ToString();
        return result;
    }

    public static int[] ToQuatArray(this ushort value, int length = 8){
        var array = new int[length];
        var index = 0;
        while(index < length){
            var n = value % 4;
            array[index++] = n;
            value >>= 2;
        }
        return array;
    }

    public static int[] ToQuatArray(this string value, int length = 8){
        var array = new int[length];
        var index = 0;
        while(length-- > 0){
            array[index++] = Convert.ToInt32(value.Substring(length, 1));
        }
        return array;
    }

    public static ushort Encode(this int[] array, int length = 8){
        ushort result = 0;
        while(length-- > 0){
            result <<= 2;
            result += (ushort)array[length];
        }
        return result;
    }

    public static ushort Encode(this string id, int length = 8){
        Debug.Assert(id.Length == length);
        ushort result = 0;
        var index = 0;
        while(index < length){
            var n = Convert.ToUInt16(id.Substring(index, 1));
            result <<= 2;
            result += n;
            index++;
        }
        return result;
    }

    public static Location[] GetLocations(this Location point){
		var points = new Location[4];
		points[0].Pos = point.Pos + point.Rot * new Vector3(-1, 0, 1) * Define.ROOM_RAD;
		points[0].Rot = point.Rot;
		points[1].Pos = point.Pos + point.Rot * new Vector3(-2, 0, 2) * Define.ROOM_RAD;
		points[1].Rot = point.Rot;
		points[2].Pos = point.Pos + point.Rot * new Vector3(-2, 0, -2) * Define.ROOM_RAD;
		points[2].Rot = point.Rot * Quaternion.Euler(0, -90, 0); 
		points[3].Pos = point.Pos + point.Rot * new Vector3(2, 0, 2) * Define.ROOM_RAD;
		points[3].Rot = point.Rot * Quaternion.Euler(0, 90, 0); 
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