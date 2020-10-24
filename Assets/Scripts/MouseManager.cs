using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public LayerMask TileMask;
    private Tile m_hovering;
    private UnitManager m_uManager;
    public Action<Tile> OnClickTile;
    public static MouseManager Instance {get; private set;} = null;
    public bool Activating{get; set;} = true;
    void Awake(){
        m_uManager = FindObjectOfType<UnitManager>();
        Instance = this;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, TileMask)){
            m_hovering?.OnLeave();
            m_hovering = hit.collider.GetComponent<Tile>();
            m_hovering?.OnHover();
        }
        if (Input.GetMouseButtonDown(0) && Activating){
            OnClickTile.Invoke(m_hovering);
        }
    }
}
