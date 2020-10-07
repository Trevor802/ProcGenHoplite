using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	private const string m_startID = "00000100";
	private Queue<Room> m_queue = new Queue<Room>();
	// private HashSet<int> m_used = new HashSet<int>();
	private Dictionary<int, bool> m_used = new Dictionary<int, bool>();
	private void Start() {
		var loc = new Location(Vector3.zero, Quaternion.identity);
		Populate(loc);
	}

	private void Populate(Location loc){
		var locs = loc.GetLocations();
		for(int i = 0; i < 4; i++){
			var r = GenRoom(locs[i], m_startID);
			var hash = locs[i].ToHash();
			if (m_used.ContainsKey(hash)){
				m_used[hash] = true;
			}
			else{
				m_used.Add(hash, false);
			}
			if (i > 0){
				m_queue.Enqueue(r);
			}
		}
	}

	private bool CanGen(Location location){
		if (location.Pos.x >= -0.1f && location.Pos.z <= 0.1f){
			return false;
		}
		var hash = location.ToHash();
		return !m_used.ContainsKey(hash) || !m_used[hash];
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.N)){
			Next();
		}
	}

	private void Next(){
		var room = m_queue.Dequeue();
		if (CanGen(room.Loc)){
			Populate(room.Loc);
		}
	}

	private Room GenRoom(Location location, string id){
		var prefab = RoomPool.LoadRoom(id);
		var obj = Instantiate<GameObject>(prefab, location.Pos, location.Rot);
		var room = obj.GetComponent<Room>();
		room.ID = id;
		room.Loc = location;
		return room;
	}
}

public struct Location{
	public Location(Vector3 p, Quaternion r){
		Pos = p;
		Rot = r;
	}
	public Vector3 			Pos;
	public Quaternion 		Rot;
}
public class RoomPool{
	private static Dictionary<string, GameObject> m_pool = new Dictionary<string, GameObject>();

	public static GameObject LoadRoom(string id){
		if (!m_pool.ContainsKey(id)){
			var obj = Resources.Load<GameObject>($"Rooms/{id}");
			if (obj is null){
				throw new InvalidOperationException("Failed to load room");
			}
			m_pool[id] = obj;
		}
		return m_pool[id];
	}
}
