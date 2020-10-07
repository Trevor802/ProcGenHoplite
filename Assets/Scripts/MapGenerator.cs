using System;
using Random = System.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapGenerator : MonoBehaviour
{
	private const string m_startID = "00000100";
	public int RandSeed;
	private Queue<Room> m_queue = new Queue<Room>();
	// private HashSet<int> m_used = new HashSet<int>();
	private Dictionary<int, bool> m_used = new Dictionary<int, bool>();
	private Dictionary<int, Room> m_record = new Dictionary<int, Room>();
	private Random m_random;
	private void Start() {
		if (RandSeed == 0){
			RandSeed = new Random().Next();
		}
		m_random = new Random(RandSeed);
		// var loc = new Location(Vector3.zero, Quaternion.identity);
		Populate(null);
	}

	private void Populate(Room baseRoom){
		var locs = baseRoom is null ? new Location(Vector3.zero, Quaternion.identity).GetLocations() : baseRoom.Loc.GetLocations();
		for(int i = 0; i < 4; i++){
			var id = GetAvailRoomID(baseRoom, Direction.Base);
			var r = GenRoom(locs[i], id);
			var hash = locs[i].ToHash(false);
			if (m_used.ContainsKey(hash)){
				m_used[hash] = true;
			}
			else{
				m_used.Add(hash, false);
			}
			if (i > 0){
				m_queue.Enqueue(r);
			}
			var _1 = locs[i].ToHash(true);
			try{
				m_record.Add(locs[i].ToHash(true), r);
			}
			catch(Exception e){
				throw e;
			}
		}
	}

	private string GetAvailRoomID(Room room, Direction d){
		if (room is null){
			return m_startID;
		}
		// var mask = room.GetMask(d);
		var availRooms = RoomPool.All;
		var rand = m_random.Next(availRooms.Length);
		var id = availRooms[rand];
		return id;
	}

	private bool CanGen(Location location){
		if (location.Pos.x >= -0.1f && location.Pos.z <= 0.1f){
			return false;
		}
		var hash = location.ToHash(false);
		return !m_used.ContainsKey(hash) || !m_used[hash];
	}

	private void Update() {
		if (Input.GetKey(KeyCode.N)){
			Next();
		}
	}

	private void Next(){
		var room = m_queue.Dequeue();
		if (CanGen(room.Loc)){
			Populate(room);
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

	private static string[] m_allRooms;
	public static string[] All{
		get{
			if (m_allRooms is null){
				var data = AssetDatabase.FindAssets("t:prefab", new[] {Define.ROOM_PATH});
				var result = new string[data.Length];
				for(int i = 0; i < data.Length; i++){
					result[i] = AssetDatabase.GUIDToAssetPath(data[i]).Substring(Define.ROOM_PATH.Length+1, 8);
				}
				m_allRooms = result;
			}
			return m_allRooms;
		}
	}
}
