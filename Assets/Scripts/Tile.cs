using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public LayerMask UnitMask;
    public LayerMask AllTile;
    public Material Empty;
    public Material HasEnemy;
    public bool Used;
    private void Awake() {
        AllTile = LayerMask.GetMask("Tile", "Block");
    }
    public void OnHover(){
        var render = GetComponent<MeshRenderer>();
        render.enabled = true;
        var unit = GetUnit();
        if (unit is null){
            render.material = Empty;
        }
        else if(unit is Enemy){
            render.material = HasEnemy;
        }
    }
    public void OnLeave(){
        var render = GetComponent<MeshRenderer>();
        render.enabled = false;
    }

    public static Tile operator + (Tile tile, Vector3 dir){
        var target = tile.transform.position + dir;
        RaycastHit hit;
        if (Physics.Linecast(target + Vector3.up, target, out hit, tile.AllTile)){
            return hit.transform.GetComponent<Tile>();
        }
        return null;
    }

    public Unit GetUnit(){
        RaycastHit hit;
        Unit unit = null;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, float.MaxValue, UnitMask)){
            unit = hit.collider.GetComponent<Unit>();
        }
        return unit;
    }
}
