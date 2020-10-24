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
        if (tile == GetTile()){
            return false;
        }
        return true;
    }

    private bool CanAttack(Tile tile){
        var dir = tile.transform.position - transform.position;
        if (dir.IsDiag()){
            return false;
        }
        if (!CanMove(tile)){
            return false;
        }
        var unit = tile.GetUnit();
        if (unit != null && unit is Enemy){
            return true;
        }
        return false;
    }

    private bool CanPush(Tile tile){
        var dir = tile.transform.position - transform.position;
        if (dir.IsDiag()){
            return false;
        }
        var unit = tile.GetUnit();
        if (unit != null && 
            unit is Enemy &&
            m_mana >= 3){
            return true;
        }
        return false;
    }
    protected override void OnEnterTile(Tile tile){
        base.OnEnterTile(tile);
        if (tile.tag == "Door" && tile.Used == false){
            var door = tile.GetComponentInParent<Door>();
            if (door.IsExit){
                UnitManager.Instance.DestroyAllUnits();
                tile.Used = true;
            }
            else{
                UnitGenerator.Instance.FillNextRoom();
                tile.Used = true;
            }
        }
    }
    public void OnClickTile(Tile tile){
        if (UnitManager.Instance.IsPlayerTurn){
            if (CanPush(tile)){
                var dir = tile.transform.position - transform.position;
                StartCoroutine(Push(tile, dir, UnitManager.Instance.AfterPlayerTurn));
            }
            else if (CanAttack(tile)){
                StartCoroutine(Attack(tile, UnitManager.Instance.AfterPlayerTurn));
            }
            else if (CanMove(tile)){
                StartCoroutine(
                    Move(transform, transform.position, tile.transform.position, MoveSpeed, UnitManager.Instance.AfterPlayerTurn)
                );
            }
        }
    }

    protected override void Die()
    {
        throw new NotImplementedException();
    }
}
