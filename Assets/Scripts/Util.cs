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
		if (xAbs > 0){
			result.Add(new Vector3(vec.x, 0, 0));
		}
		if (zAbs > 0){
			result.Add(new Vector3(0, 0, vec.z));
		}
		return result;
    }
	
	public static Tile GetTile(Vector3 pos){
		var allTile = LayerMask.GetMask("Tile", "Block");
		RaycastHit hit;
		if (Physics.Linecast(pos + Vector3.up, pos, out hit, allTile)){
			return hit.transform.GetComponent<Tile>();
		}
		return null;
	}
}