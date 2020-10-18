using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public LayerMask UnitMask;
    public Material Empty;
    public Material HasEnemy;
    public void OnHover(){
        var render = GetComponent<MeshRenderer>();
        render.enabled = true;
        var unit = GetUnitInside();
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

    public Unit GetUnitInside(){
        RaycastHit hit;
        Unit unit = null;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, float.MaxValue, UnitMask)){
            unit = hit.collider.GetComponent<Unit>();
        }
        return unit;
    }
}
