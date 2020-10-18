using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float MoveSpeed = 10f;
    private Action m_callback;
    private List<IUnitAction> m_actions = new List<IUnitAction>();
    private void Awake() {
        m_actions.Add(new IMoveAction());
        m_actions.Add(new IAttackAction());
    }
    // public IUnitAction GetRandActins(Unit unit){
    //     var actions = new List<IUnitAction>();
    //     foreach(var a in m_actions){
    //         if (a.CanAct(unit))
    //             actions.Add(a);
    //     }
    // }
    internal virtual void TakeAction(Action callback){}
    protected IEnumerator MoveTo(Tile tile){
        transform.LookAt(tile.transform.position, Vector3.up);
        yield return MoveFromTo(transform, transform.position, tile.transform.position, MoveSpeed);
        m_callback?.Invoke();
    }
    protected IEnumerator Attack(Unit unit){
        yield return null;
        m_callback?.Invoke();
    }

    protected IEnumerator Push(Unit unit){
        yield return null;
        m_callback?.Invoke();
    }

    protected IEnumerator Throw(Unit unit){
        yield return null;
        m_callback?.Invoke();
    }
    private IEnumerator MoveFromTo(Transform objectToMove, Vector3 a, Vector3 b, float speed){
        float step = (speed / (a - b).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f) {
            t += step; // Goes from 0 to 1, incrementing by step each time
            objectToMove.position = Vector3.Lerp(a, b, t); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }
        objectToMove.position = b;
    }
}
