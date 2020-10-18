using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Door NDoor;
    public Door SDoor;
    public Door WDoor;
    public Door EDoor;
    private List<Tile> m_floors = new List<Tile>();
    private List<Unit> m_enemies = new List<Unit>();
    private UnitGenerator m_uGen;
    public void SetupTile(){
        m_floors.AddRange(GetComponentsInChildren<Tile>().Where(x => x.tag == "Floor"));
        m_uGen = FindObjectOfType<UnitGenerator>();
    }
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
                if (NDoor)
                    local = NDoor.transform.localPosition;
                break;
            case Orientation.North:
                if (SDoor)
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

    public void GenerateUnits(int depth){
        SetCamera();
        SetupTile();
        var list = m_floors.OrderBy(x => Random.Range(0, int.MaxValue)).Take(depth);
        foreach(var f in list){
            var prefab = Random.Range(0, 2) == 0 ? m_uGen.WarriorPrefab : m_uGen.ArcherPrefab;
            var enemy = Instantiate(prefab, f.transform.position, Quaternion.identity);
            m_enemies.Add(enemy);
        }
    }

    public void SetCamera(){
        var pos = Camera.main.transform.position;
        pos.x = transform.position.x + 4f;
        pos.z = transform.position.z - 4f;
        Camera.main.transform.position = pos;
    }
}
