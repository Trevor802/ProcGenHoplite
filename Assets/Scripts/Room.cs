using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    // public string ID{get; set;} = "00000000";
    public int[] ID = new int[8];
    public Location Loc{get; set;}

    public int[] GetMask(Direction direction){
        var result = Enumerable.Repeat(-1, 8).ToArray();
        switch (direction){
            case Direction.Base:
            case Direction.Front:
                result[3] = ID[0] == 0 ? 0 : -1;
                result[4] = ID[7] == 0 ? 0 : -1;
                break;
            case Direction.Left:
                result[3] = ID[6] == 0 ? 0 : -1;
                result[4] = ID[5] == 0 ? 0 : -1;
                break;
            case Direction.Right:
                result[3] = ID[2] == 0 ? 0 : -1;
                result[4] = ID[1] == 0 ? 0 : -1;
                break;
        }
        return result;
    }
}
