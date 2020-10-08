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
			var id = GetAvailRoomID(baseRoom, (Direction)i);
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
			m_record.Add(locs[i].ToHash(true), r);
		}
	}

	private ushort GetAvailRoomID(Room room, Direction d){
		if (room is null){
			return m_startID.Encode();
		}
		var mask = room.GetMask(d);
		var availRooms = RoomPool.All.ApplyMask(mask);
		var rand = m_random.Next(availRooms.Length);
		var id = availRooms[rand];
		return id.Encode();
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

	private Room GenRoom(Location location, ushort id){
		var prefab = RoomPool.LoadRoom(id.ToQuatArray());
		var obj = Instantiate<GameObject>(prefab, location.Pos, location.Rot);
		var room = obj.GetComponent<Room>();
		room.ID = id.ToQuatArray();
		room.Loc = location;
		return room;
	}
}

public static class RoomPool{

	static RoomPool(){
		var data = AssetDatabase.FindAssets("t:prefab", new[] {Define.ROOM_PATH});
		var result = new string[data.Length];
		for(int i = 0; i < data.Length; i++){
			result[i] = AssetDatabase.GUIDToAssetPath(data[i]).Substring(Define.ROOM_PATH.Length+1, 8);
		}
		m_allRooms = result;
	}
	private static Dictionary<string, GameObject> m_pool = new Dictionary<string, GameObject>();

	public static GameObject LoadRoom(int[] array){
		var id = array.ToQuatStr();
		if (!m_pool.ContainsKey(id)){
			var obj = Resources.Load<GameObject>($"Rooms/{id}");
			if (obj is null){
				throw new InvalidOperationException("Failed to load room");
			}
			m_pool[id] = obj;
		}
		return m_pool[id];
	}

	public static bool CheckMask(int[] source, int[] mask){
		Debug.Assert(source.Length == 8);
		Debug.Assert(mask.Length == 8);
		for(int i = 0; i < 8; i++){
			if (source[i] != mask[i] && mask[i] != -1){
				return false;
			}
		}
		return true;
	}

	public static string[] ApplyMask(this string[] all, int[] mask){
		var result = new string[all.Length];
		var index = 0;
		foreach(var r in all){
			if (CheckMask(r.ToQuatArray(), mask)){
				result[index++] = r;
			}
		}
		Array.Resize<string>(ref result, index);
		return result;
	}

	private static string[] m_allRooms;
	public static string[] All => m_allRooms;
}
