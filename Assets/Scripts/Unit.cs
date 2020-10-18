using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour
{
    protected UnitManager m_uManager;
    public float MoveSpeed = 10f;
    private Action m_callback;
    private List<IUnitAction> m_actions = new List<IUnitAction>();
    protected IUnitAction m_nullAction = new INullAction();
    protected IMoveAction m_moveAction = new IMoveAction();
    protected IAttackAction m_attackAction = new IAttackAction();
    protected virtual void Awake() {
        m_uManager = FindObjectOfType<UnitManager>();
        m_actions.Add(m_moveAction);
        m_actions.Add(m_attackAction);
    }
    public IUnitAction GetRandActins(Unit unit){
        var actions = new List<IUnitAction>();
        foreach(var a in m_actions){
            if (a.CanAct(unit))
                actions.Add(a);
        }
        if (actions.Count == 0)
            return m_nullAction;
        int id = Random.Range(0, actions.Count);
        return actions[id];
    }
    internal virtual void TakeAction(IUnitAction actionToTake, Action callback){}
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
