using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public static Player Instance {get; private set;}
    private int m_mana = 3;
    protected override void Awake() {
        base.Awake();
        Instance = this;
    }
    private void Start() {
        MouseManager.Instance.OnClickTile += this.OnClickTile;
    }
    internal override void TakeAction(Action callback)
    {
    }

    private bool CanMove(Tile tile){
        var dir = tile.transform.position - transform.position;
        if (dir.IsDiag()){
            return false;
        }
        if ((dir).magnitude > m_mana){
            return false;
        }
        return true;
    }

    private bool CanAttack(Tile tile){
        if (CanMove(tile)){
            var unit = tile.GetUnitInside();
            if (unit != null && unit is Enemy){
                return true;
            }
        }
        return false;
    }

    public void OnClickTile(Tile tile){
        if (UnitManager.Instance.IsPlayerTurn){
            if (CanAttack(tile)){
                StartCoroutine(Attack(tile, UnitManager.Instance.AfterPlayerTurn));
            }
            else if (CanMove(tile)){
                StartCoroutine(
                    Move(transform, transform.position, tile.transform.position, MoveSpeed, UnitManager.Instance.AfterPlayerTurn)
                );
            }
        }
    }
}
