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
		points[(int)Direction.Center].Pos = point.Pos + point.Rot * new Vector3(-1, 0, 1) * Define.ROOM_RAD;
		points[(int)Direction.Center].Rot = point.Rot;
		points[(int)Direction.Front].Pos = point.Pos + point.Rot * new Vector3(-2, 0, 2) * Define.ROOM_RAD;
		points[(int)Direction.Front].Rot = point.Rot;
		points[(int)Direction.Left].Pos = point.Pos + point.Rot * new Vector3(-2, 0, -2) * Define.ROOM_RAD;
		points[(int)Direction.Left].Rot = point.Rot * Quaternion.Euler(0, -90, 0); 
		points[(int)Direction.Right].Pos = point.Pos + point.Rot * new Vector3(2, 0, 2) * Define.ROOM_RAD;
		points[(int)Direction.Right].Rot = point.Rot * Quaternion.Euler(0, 90, 0); 
		return points;
	}

    public static Location GetLocationByDir(this Location baseLoc, Direction dir){
        var pos = Vector3.zero;
        var rot = Quaternion.identity;
        switch(dir){
            case Direction.Center:
                rot = baseLoc.Rot;
                pos = baseLoc.Pos + baseLoc.Rot * new Vector3(-1, 0, 1) * Define.ROOM_RAD;
                break;
            case Direction.Front:
                rot = baseLoc.Rot;
                pos = baseLoc.Pos + baseLoc.Rot * new Vector3(-2, 0, 2) * Define.ROOM_RAD;
                break;
            case Direction.Left:
                rot = baseLoc.Rot * Quaternion.Euler(0, -90, 0);
                pos = baseLoc.Pos + baseLoc.Rot * new Vector3(-2, 0, -2) * Define.ROOM_RAD;
                break;
            case Direction.Right:
                rot = baseLoc.Rot * Quaternion.Euler(0, 90, 0);
                pos = baseLoc.Pos + baseLoc.Rot * new Vector3(2, 0, 2) * Define.ROOM_RAD;
                break;
        }
        return new Location(pos, rot); 
    }

    public static Location GetBaseLocation(this Location loc, Direction dir){
        var pos = Vector3.zero;
        var rot = Quaternion.identity;
        switch(dir){
            case Direction.Center:
                rot = loc.Rot;
                pos = loc.Pos - rot * new Vector3(-1, 0, 1) * Define.ROOM_RAD;
                break;
            case Direction.Front:
                rot = loc.Rot;
                pos = loc.Pos - rot * new Vector3(-2, 0, 2) * Define.ROOM_RAD;
                break;
            case Direction.Left:
                rot = loc.Rot * Quaternion.Euler(0, 90, 0);
                pos = loc.Pos - rot * new Vector3(-2, 0, -2) * Define.ROOM_RAD;
                break;
            case Direction.Right:
                rot = loc.Rot * Quaternion.Euler(0, -90, 0);
                pos = loc.Pos - rot * new Vector3(2, 0, 2) * Define.ROOM_RAD;
                break;
        }
        return new Location(pos, rot);
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

    public static int ToHash(this Location location, Orientation orientation){
        var hash = location.ToHash(false);
        hash += (int)orientation * Define.MAP_SIZE_SQUARE;
        return hash;
    }

    public static bool IsAtLimbo(this Location location){
        if (location.Pos.x >= -Define.LIMBO_CHECK && location.Pos.z <= Define.LIMBO_CHECK){
			return true;
        }
        return false;
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

    public static Orientation Rotate(this Orientation orientation, Direction direction){
        var add = 0;
        if (direction == Direction.Left){
            add = -1;
        }
        else if (direction == Direction.Right){
            add = 1;
        }
        var result = ((int)orientation + add + 4) % 4;
        return (Orientation)result;
    }
}