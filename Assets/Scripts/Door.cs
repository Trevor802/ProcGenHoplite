using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject Mesh;
    public Orientation Orientation;
    public bool Connected = false;
    public void Orient(Orientation orientation){
        Orientation = orientation;
        var q = Quaternion.identity;
        switch(orientation){
            case Orientation.East:
                q = Quaternion.Euler(0, 90, 0);
                break;
            case Orientation.West:
                q = Quaternion.Euler(0, -90, 0);
                break;
            case Orientation.North:
                break;
            case Orientation.South:
                q = Quaternion.Euler(0, 180, 0);
                break;
        }
        Mesh.transform.rotation = q;
    }



    public Vector3 NextDoorPos(){
        Vector3 result = transform.position;
        switch(Orientation){
            case Orientation.East:
                result += new Vector3(1, 0, 0);
                break;
            case Orientation.West:
                result += new Vector3(-1, 0, 0);
                break;
            case Orientation.North:
                result += new Vector3(0, 0, 1);
                break;
            case Orientation.South:
                result += new Vector3(0, 0, -1);
                break;
        }
        return result;
    }
}
