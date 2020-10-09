using System;
using System.Linq;
using Random = System.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapGenerator : MonoBehaviour
{
	private const string m_startID = "01000000";
	public int RandSeed;
	public bool Surround = false;
	public int NumRoom = 10;
	private int m_countRoom = 0;
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
		Populate(null);
	}

	private void Populate(Room baseRoom, bool genNullRoom = false){
		var locs = baseRoom is null ? new Location(Vector3.zero, Quaternion.identity).GetLocations() : baseRoom.Loc.GetLocations();
		for(int i = 0; i < 4; i++){
			ushort id = 0;
			if (!genNullRoom){
				id = GetAvailRoomID(locs[i], (Direction)i);
			}
			// if (id == 0){
			// 	continue;
			// }
			var r = GenRoom(locs[i], id);
			var hash = locs[i].ToHash(false);
			if (m_used.ContainsKey(hash)){
				m_used[hash] = true;
			}
			else{
				m_used.Add(hash, false);
			}
			if (i > 0 && !genNullRoom){
				m_queue.Enqueue(r);
			}
			m_record.Add(locs[i].ToHash(true), r);
			m_countRoom++;
		}
	}

	private List<Room> RoomsAtPos(Location loc){
		var result = new List<Room>();
		for(int i = 0; i < 4; i++){
			var hash = loc.ToHash((Orientation)i);
			if (m_record.ContainsKey(hash)){
				result.Add(m_record[hash]);
			}
		}
		return result;
	}

	private ushort GetAvailRoomID(Location loc, Direction d){
		// Gen mask
		var mask = Enumerable.Repeat(-1, 8).ToArray();
		var lHash = 0;
		var rHash = 0;
		var cHash = 0;
		var bLoc = loc.GetBaseLocation(d);
		var bHash = bLoc.ToHash(true);
		var orient = loc.Rot.ToOrientation();
		if (d == Direction.Center){
			if (m_record.ContainsKey(bHash)){
				mask[4] = m_record[bHash].ID[7];
				mask[3] = m_record[bHash].ID[0];
			}
			else{
				return m_startID.Encode();
			}
		}
		else if(d == Direction.Left){
			lHash = loc.ToHash(orient.Rotate(Direction.Left));
			if (m_record.ContainsKey(lHash)){
				mask[0] = m_record[lHash].ID[7];
				mask[1] = m_record[lHash].ID[6];
			}
			else if(loc.IsAtLimbo()){
				mask[0] = 0;
				mask[1] = 0;
			}
			rHash = loc.ToHash(orient.Rotate(Direction.Right));
			if (m_record.ContainsKey(rHash)){
				mask[7] = m_record[rHash].ID[0];
				mask[6] = m_record[rHash].ID[1];
			}
			else if(loc.IsAtLimbo()){
				mask[7] = 0;
				mask[6] = 0;
			}
			if (m_record.ContainsKey(bHash)){
				mask[5] = m_record[bHash].ID[6];
			}
			else if(bLoc.IsAtLimbo()){
				mask[5] = 0;
			}
			cHash = bLoc.GetLocationByDir(Direction.Center).ToHash(true);
			if (m_record.ContainsKey(cHash)){
				mask[3] = m_record[cHash].ID[6];
				mask[4] = m_record[cHash].ID[5];
			}
		}
		else if(d == Direction.Right){
			lHash = loc.ToHash(orient.Rotate(Direction.Left));
			if (m_record.ContainsKey(lHash)){
				mask[0] = m_record[lHash].ID[7];
				mask[1] = m_record[lHash].ID[6];
			}
			else if(loc.IsAtLimbo()){
				mask[0] = 0;
				mask[1] = 0;
			}
			rHash = loc.ToHash(orient.Rotate(Direction.Right));
			if (m_record.ContainsKey(rHash)){
				mask[7] = m_record[rHash].ID[0];
				mask[6] = m_record[rHash].ID[1];
			}
			else if(loc.IsAtLimbo()){
				mask[7] = 0;
				mask[6] = 0;
			}
			if (m_record.ContainsKey(bHash)){
				mask[2] = m_record[bHash].ID[1];
			}
			else if(bLoc.IsAtLimbo()){
				mask[2] = 0;
			}
			cHash = bLoc.GetLocationByDir(Direction.Center).ToHash(true);
			if (m_record.ContainsKey(cHash)){
				mask[3] = m_record[cHash].ID[2];
				mask[4] = m_record[cHash].ID[1];
			}
		}
		else if(d == Direction.Front){
			lHash = bLoc.GetLocationByDir(Direction.Left).ToHash(true);
			rHash = bLoc.GetLocationByDir(Direction.Right).ToHash(true);
			cHash = bLoc.GetLocationByDir(Direction.Center).ToHash(true);
			if (m_record.ContainsKey(lHash)){
				mask[5] = m_record[lHash].ID[2];
			}
			if (m_record.ContainsKey(cHash)){
				mask[4] = m_record[cHash].ID[7];
				mask[3] = m_record[cHash].ID[0];
			}
			if (m_record.ContainsKey(rHash)){
				mask[2] = m_record[rHash].ID[5];
			}
			if (loc.IsAtLimbo()){
				if (loc.Pos.x >= -0.1f){
					mask[7] = 0;
					mask[6] = 0;
				}
				else{
					mask[0] = 0;
					mask[1] = 0;
				}
			}
		}
		// Choose rooms based on mask
		var availRooms = RoomPool.All.ApplyMask(mask);
		if (availRooms.Length == 0){
			Debug.LogWarning($"No room available for mask {mask.ToQuatStr()}");
			return Define.NULL_ROOM.Encode();
		}
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
		if (NumRoom == 0){
			if (Input.GetKey(KeyCode.N)){
				Next();
			}
			if (Input.GetKeyDown(KeyCode.Space)){
				Next();
			}
		}
		else{
			if (m_countRoom < NumRoom){
				Next();
			}
			else if (Surround){
				NextSurround();
			}
		}
	}

	private void NextSurround(){
		var room = m_queue.Dequeue();
		Populate(room, true);
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
			if (source[i] != mask[i] && mask[i] != -1 && (source[i] == 0 || mask[i] == 0)){
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
