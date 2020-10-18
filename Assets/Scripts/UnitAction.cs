using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public interface IUnitAction{
    bool CanAct(Unit unit);
    // IEnumerator Act(List<object> parameters, Action callback);
}

public class IMoveAction : IUnitAction
{
    public IEnumerator Act(Transform objectToMove, Vector3 a, Vector3 b, float speed, Action callback)
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
        callback();
    }

    public bool CanAct(Unit unit)
    {
        if (unit is Player player){
            return true;
        }
        else if (unit is Enemy enemy){

        }
        return false;
    }
}

public class IAttackAction : IUnitAction
{
    public IEnumerator Act(List<object> parameters, Action callback)
    {
        yield return null;
        callback?.Invoke();
    }

    public bool CanAct(Unit unit)
    {
        if (unit is Player player){

        }
        else if (unit is Enemy enemy){

        }
        return false;
    }
}

class INullAction : IUnitAction
{
    public IEnumerator Act(List<object> parameters, Action callback)
    {
        yield return null;
        callback?.Invoke();
    }

    public bool CanAct(Unit unit)
    {
        return true;
    }
}