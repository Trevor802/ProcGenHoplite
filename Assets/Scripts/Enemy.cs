using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
public class Enemy : Unit
{
    private bool m_canSeePlayer;
    private Tile m_nextTile;
    private bool m_canAttackPlayer;
    private bool m_canThrowPlayer;
    private static readonly Vector3[] m_dirs = new Vector3[4]{Vector3.forward, Vector3.back, Vector3.left, Vector3.right};
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
            Debug.Log("Attack");
            callback();
        }
        else if (m_canThrowPlayer){
            // throw
            Debug.Log("Throw");
            callback();
        }
        else if (m_canSeePlayer){
            StartCoroutine(Move(transform, transform.position, m_nextTile.transform.position, MoveSpeed, callback));
        }
        else{
            Debug.Log("Random walk");
            var tile = GetAvailTile(m_dirs.ToList());
            StartCoroutine(Move(transform, transform.position, tile.transform.position, MoveSpeed, callback));
        }
    }

    protected Tile GetAvailTile(List<Vector3> list){
        list.Shuffle();
        foreach(var d in list){
            var tile = TryMoveDir(d);
            if (tile != null){
                return tile;
            }
        }
        throw new InvalidOperationException();
    }

    protected Tile TryMoveDir(Vector3 dir){
        if (dir.IsDiag()){
            return null;
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, float.MaxValue, FindPlayerMask)){
            if (BlockMask == (BlockMask | 1 << hit.transform.gameObject.layer)){
                return null;
            }
            else if (TileMask == (TileMask | 1 << hit.transform.gameObject.layer)){
                return hit.transform.GetComponent<Tile>();
            }
        }
        return null;
    }

    private void ClearFlags(){
        m_canSeePlayer = false;
        m_canAttackPlayer = false;
        m_canThrowPlayer = false;
        m_nextTile = null; 
    }

    protected void TrackPlayer(){
        ClearFlags();
        var direction = Player.Instance.transform.position - transform.position;
        if (Physics.Linecast(transform.position, Player.Instance.transform.position, BlockMask)){
            return;
        }
        else{
            m_canSeePlayer = true;
            if (!direction.IsDiag()){
                var dist = Vector3.Distance(In().transform.position, Player.Instance.In().transform.position);
                if (Util.NrE(dist, 2)){
                    m_canThrowPlayer = true;
                }
                else if (Util.NrE(dist, 1)){
                    m_canAttackPlayer = true;
                }
            }
            var dirs = direction.Split();
            m_nextTile = GetAvailTile(dirs);
        }
    }
}