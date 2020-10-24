using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float Delay = 1f;
    void Start()
    {
        Invoke("Suicide", Delay);
    }
    private void Suicide(){
        Destroy(gameObject);
    }
}
