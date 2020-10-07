using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public string ID{get; set;} = "00000000";
    public Location Loc{get; set;}

    public string GetSockets(Direction direction){
        var result = "";
        switch (direction){
            case Direction.Front:
                result += ID[0] + ID[7];
                break;
            case Direction.Left:
                result += ID[2] + ID[1];
                break;
            case Direction.Right:
                result += ID[6] + ID[5];
                break;
        }
        return result;
    }
}
