using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public static Player Instance {get; private set;}
    protected override void Awake() {
        base.Awake();
        Instance = this;
    }
    internal override void TakeAction(IUnitAction actionToTake, Action callback)
    {
        base.TakeAction(actionToTake, callback);
    }
    public bool MoveTo(Tile tile){
        if (m_moveAction.CanAct(this)){
            StartCoroutine(
                m_moveAction.Act(transform, transform.position, tile.transform.position, MoveSpeed, m_uManager.AfterPlayerTurn)
            );
            return true;
        }
        return false;
    }
}
