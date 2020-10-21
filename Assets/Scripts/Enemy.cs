using System;
using UnityEngine;
public class Enemy : Unit
{
    private static int FindPlayerMask ;
    private static int BlockMask ;
    private static  int TileMask ;
    private bool m_canSeePlayer;
    private Tile m_nextTile;
    private bool m_canAttackPlayer;
    private bool m_canThrowPlayer;
    protected virtual void Awake() {
        base.Awake();
        UnitManager.Instance.RegisterUnit(this);
        FindPlayerMask = LayerMask.GetMask("Tile", "Block");
        BlockMask = LayerMask.GetMask("Block");
        TileMask = LayerMask.GetMask("Tile");
    }

    internal override void TakeAction(Action callback)
    {
        TrackPlayer();
        if (m_canAttackPlayer){
            // attack first
        }
        else if (m_canThrowPlayer){
            // throw
        }
        else if (m_canSeePlayer){
            StartCoroutine(Move(transform, transform.position, m_nextTile.transform.position, MoveSpeed, callback));
        }
        else{
            Debug.Log("No where to go");
            callback();
        }
    }

    protected void TrackPlayer(){
        RaycastHit[] hits;
        var direction = Player.Instance.transform.position - transform.position;
        hits = Physics.RaycastAll(transform.position, direction, float.MaxValue, FindPlayerMask);
        Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
        m_canSeePlayer = true;
        bool isNextTileSet = false;
        foreach(var hit in hits){
            if (hit.transform.gameObject.layer == BlockMask){
                m_canSeePlayer = false;
            }
            else if (!isNextTileSet && TileMask == (TileMask | 1 << hit.transform.gameObject.layer)){
                m_nextTile = hit.transform.GetComponent<Tile>();
                isNextTileSet = true;
            }
        }
    }
}