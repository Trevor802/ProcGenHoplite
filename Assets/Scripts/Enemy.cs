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
    protected bool m_isArcher = false;
    private static readonly Vector3[] m_dirs = new Vector3[4]{Vector3.forward, Vector3.back, Vector3.left, Vector3.right};
    protected virtual void Awake() {
        base.Awake();
        UnitManager.Instance.RegisterUnit(this);
        FindPlayerMask = LayerMask.GetMask("Tile", "Block");
        BlockMask = LayerMask.GetMask("Block");
        TileMask = LayerMask.GetMask("Tile");
        m_health = 1;
    }

    protected override void OnEnterTile(Tile tile)
    {
        base.OnEnterTile(tile);
        if (tile.tag == "Door"){
            Die();
        }
    }

    internal override void TakeAction(Action callback)
    {
        TrackPlayer();
        if (m_canAttackPlayer && !m_isArcher){
            // attack first
            StartCoroutine(Attack(Player.Instance.GetTile(), callback));
        }
        else if (m_canThrowPlayer && m_isArcher){
            // throw
            StartCoroutine(Throw(Player.Instance.GetTile(), callback));
        }
        else if (m_canSeePlayer && m_nextTile != null){
            StartCoroutine(Move(transform, transform.position, m_nextTile.transform.position, MoveSpeed, callback));
        }
        else{
            Debug.Log("Random walk");
            var tile = GetAvailTile(m_dirs.ToList());
            if (tile is null){
                Debug.Log("Nothing to do");
                callback();
            }
            else{
                StartCoroutine(Move(transform, transform.position, tile.transform.position, MoveSpeed, callback));
            }
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
        return null;
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
                var tile = hit.transform.GetComponent<Tile>();
                if (tile.GetUnit() != null){
                    return null;
                }
                if (tile.tag == "Door" || tile.tag == "Lava"){
                    return null;
                }
                return tile;
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
                var dist = Vector3.Distance(GetTile().transform.position, Player.Instance.GetTile().transform.position);
                if (dist >= DEF.TRW_DIST_MIN){
                    m_canThrowPlayer = true;
                    return;
                }
                else if (Util.NrE(dist, DEF.ATK_DIST)){
                    m_canAttackPlayer = true;
                    return;
                }
            }
            var dirs = direction.Split();
            m_nextTile = GetAvailTile(dirs);
        }
    }

    protected override void Die()
    {
        Debug.Log($"{name} die");
        UnitManager.Instance.DeregisterUnit(this);
        Destroy(gameObject);
    }
}