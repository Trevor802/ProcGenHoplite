using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Unit : MonoBehaviour
{
    protected static int FindPlayerMask ;
    protected static int BlockMask ;
    protected static  int TileMask ;
    public float MoveSpeed = 10f;
    protected int m_health = 10;
    private Action m_callback;
    protected virtual void Awake() {
    }
    internal abstract void TakeAction(Action callback);

    protected abstract void Die();

    public virtual void Damage(int value){
        m_health -= value;
        if (m_health <= 0){
            Die();
        }
        Debug.Log($"{name} get damaged, {m_health} health left");
    }

    protected IEnumerator Move(Transform objectToMove, Vector3 a, Vector3 b, float speed, Action callback)
    {
        objectToMove.LookAt(b, Vector3.up);
        float step = (speed / (a - b).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f) {
            t += step; // Goes from 0 to 1, incrementing by step each time
            objectToMove.position = Vector3.Lerp(a, b, t); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }
        objectToMove.position = b;
        OnEnterTile(Util.GetTile(b));
        callback();
    }

    protected virtual void OnEnterTile(Tile tile){
        if (tile.gameObject.tag == "Lava"){
            Die();
            return;
        }
    }

    protected IEnumerator Attack(Tile tile, Action callback){
        Debug.Log($"{name} attack {tile.GetUnit().name}");
        tile.GetUnit().Damage(1);
        transform.LookAt(tile.transform.position, Vector3.up);
        yield return null;
        ParticleManager.Instance.PlayParticle(tile.transform.position, tile.transform.position - transform.position, ParticleManager.Instance.AttackP);
        callback();
    }

    protected virtual IEnumerator Throw(Tile tile, Action callback){
        Debug.Log($"{name} throw {tile.GetUnit().name}");
        tile.GetUnit().Damage(1);
        transform.LookAt(tile.transform.position, Vector3.up);
        yield return null;
        ParticleManager.Instance.PlayParticle(transform.position, tile.transform.position - transform.position, ParticleManager.Instance.ThrowP);
        callback();
    }

    protected IEnumerator Push(Tile tile, Vector3 dir, Action callback){
        Debug.Log($"{name} push {tile.GetUnit().name}");
        var unit = tile.GetUnit();
        var againstTile = tile + dir;
        var againstUnit = againstTile?.GetUnit();
        if (againstTile is null){
            throw new InvalidOperationException("Cannot push towards null tile");
        }
        // Against the wall, stop recursion
        if (BlockMask == (BlockMask | 1 << againstTile.gameObject.layer)){
            unit.Damage(DEF.PSH_WAL_DMG);
            callback();
        }
        else{
            if (againstUnit is null){
                yield return Move(unit.transform, unit.transform.position, unit.transform.position + dir, MoveSpeed, callback);
            }
            else{
                yield return unit.Push(againstTile, dir, callback);
            }
        }
    }

    public Tile GetTile(){
        RaycastHit hit;
        Tile tile = null;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, float.MaxValue, TileMask)){
            tile = hit.collider.GetComponent<Tile>();
        }
        return tile;
    }
}
