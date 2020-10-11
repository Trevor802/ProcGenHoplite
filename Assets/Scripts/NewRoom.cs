using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoom : MonoBehaviour
{
    public Door NDoor;
    public Door SDoor;
    public Door WDoor;
    public Door EDoor;
    public GameObject WallPrefab;

    public Vector3 FindRoomPos(Orientation orientation, Vector3 targetPos){
        Vector3 local = Vector3.zero;
        switch(orientation){
            case Orientation.East:
                if (WDoor)
                    local = WDoor.transform.localPosition;
                break;
            case Orientation.West:
                if (EDoor)
                    local = EDoor.transform.localPosition;
                break;
            case Orientation.South:
                if (SDoor)
                    local = NDoor.transform.localPosition;
                break;
            case Orientation.North:
                if (NDoor)
                    local = SDoor.transform.localPosition;
                break;
        }
        return targetPos - local;
    }

    private Door GetDoor(Orientation orientation){
        switch(orientation){
            case Orientation.East:
                return EDoor;
            case Orientation.West:
                return WDoor;
            case Orientation.South:
                return SDoor;
            case Orientation.North:
                return NDoor;
        }
        return null;
    }

    private void SetDoor(Orientation orientation, Door door){
        switch(orientation){
            case Orientation.East:
                EDoor = door;
                break;
            case Orientation.West:
                WDoor = door;
                break;
            case Orientation.South:
                SDoor = door;
                break;
            case Orientation.North:
                NDoor = door;
                break;
        }
    }

    public void ReplaceDoorWithWall(Orientation orientation, GameObject prefab){
        var oldObject = GetDoor(orientation);
        var newObject = Instantiate<GameObject>(prefab);
        // -- set up "undo" features for the new prefab, like setting up the old transform
        newObject.transform.parent = oldObject.transform.parent;
        newObject.transform.localPosition = oldObject.transform.localPosition;
        newObject.transform.localRotation = oldObject.transform.localRotation;
        newObject.transform.localScale = oldObject.transform.localScale;
        newObject.transform.SetSiblingIndex(oldObject.transform.GetSiblingIndex());
        // -- now delete the old prefab
        Destroy(oldObject.gameObject);
        SetDoor(orientation, null);
    }

    public void CleanUnconnectedDoors(){
        if (NDoor && !NDoor.Connected){
            ReplaceDoorWithWall(Orientation.North, WallPrefab);
        }
        if (SDoor && !SDoor.Connected){
            ReplaceDoorWithWall(Orientation.South, WallPrefab);
        }
        if (WDoor && !WDoor.Connected){
            ReplaceDoorWithWall(Orientation.West, WallPrefab);
        }
        if (EDoor && !EDoor.Connected){
            ReplaceDoorWithWall(Orientation.East, WallPrefab);
        }
    }
}
