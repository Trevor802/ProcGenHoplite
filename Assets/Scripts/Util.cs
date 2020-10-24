using System.Collections.Generic;
using UnityEngine;
public static class Util{

	public static void Shuffle<T>(this IList<T> list){
		int n = list.Count;
		while (n > 1) {
			n--;  
			int k = Random.Range(0, n+1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
	public static bool NrE(float a, float b){
		return Mathf.Abs(a - b) < 0.001f;
	}

	public static bool IsDiag(this Vector3 vec){
		if (vec.x != 0 && vec.z != 0){
            return true;
        }
		return false;
	}

    public static List<Vector3> Split(this Vector3 vec){
        var result = new List<Vector3>();
		var xAbs = Mathf.Abs(vec.x);
		var zAbs = Mathf.Abs(vec.z);
        if (NrE(xAbs, zAbs)){
			result.Add(new Vector3(vec.x, 0, 0));
			result.Add(new Vector3(0, 0, vec.z));
		}
		else if (xAbs > zAbs){
			result.Add(new Vector3(vec.x, 0, 0));
		}
		else{
			result.Add(new Vector3(0, 0, vec.z));
		}
		return result;
    }
	// private static IEnumerator MoveFromTo(Transform objectToMove, Vector3 a, Vector3 b, float speed){
    //     float step = (speed / (a - b).magnitude) * Time.fixedDeltaTime;
    //     float t = 0;
    //     while (t <= 1.0f) {
    //         t += step; // Goes from 0 to 1, incrementing by step each time
    //         objectToMove.position = Vector3.Lerp(a, b, t); // Move objectToMove closer to b
    //         yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
    //     }
    //     objectToMove.position = b;
    // }
}