using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Unit : MonoBehaviour
{
    public float MoveSpeed = 10f;
    private Action m_callback;
    protected virtual void Awake() {
    }
    internal abstract void TakeAction(Action callback);

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
        callback();
    }

    protected IEnumerator Attack(Tile tile, Action callback){
        yield return null;
        callback();
    }
}
