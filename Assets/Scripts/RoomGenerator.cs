using System.Collections;
using UnityEngine.UI;
using System;
using Random = System.Random;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public Text SeedUI;
	public TextAsset[] MapFileList;
	[System.Serializable]
	public struct CharTile{
		public char C;
		public GameObject Tile;
	}
	public GameObject DefaultTile;
	public CharTile[] Prefabs;
	private Dictionary<char, GameObject> map;
	private Vector3 m_currentPos;
	public int RandomSeed = 0;
	private Random m_rand;
	public int RoomsToVictory = 10;
	private int m_count;
	private bool m_generating = false;
	private List<NewRoom> m_wRooms = new List<NewRoom>();
	private List<NewRoom> m_eRooms = new List<NewRoom>();
	private List<NewRoom> m_nRooms = new List<NewRoom>();
	private List<NewRoom> m_sRooms = new List<NewRoom>();
	private List<NewRoom> m_oRooms = new List<NewRoom>();
	private Stack<Door> m_doors = new Stack<Door>();
	// private Queue<Door> m_doors = new Queue<Door>();
	private List<NewRoom> m_realRooms = new List<NewRoom>();
	void Start()
	{
		map = new Dictionary<char, GameObject>();
		foreach(var p in Prefabs){
			map.Add(p.C, p.Tile);
		}
		if (RandomSeed == 0){
			RandomSeed = new Random().Next();
		}
		m_rand = new Random(RandomSeed);
		SeedUI.text = $"Seed:{RandomSeed.ToString()}";
		foreach(var file in MapFileList){
			ParseStr(file.text, file.name);
		}
		m_generating = true;
	}

	private void Update() {
		if (m_generating){
			if (m_count < RoomsToVictory){
				Next();
			}
			else{
				m_generating = false;
				AfterGen();
			}
		}
	}

	private void AfterGen(){
		foreach(var r in m_realRooms){
			r.CleanUnconnectedDoors();
		}
	}

	private void Next(){
		NewRoom room = null;
		Vector3 pos;
		bool first = m_doors.Count == 0;
		var o = Orientation.North;
		if (!first){
			var door = m_doors.Pop();
			door.Connected = true;
			o = door.Orientation;
			if (m_count == RoomsToVictory - 1){
				room = m_oRooms[1];
			}
			else{
				room = FindRoom(o);
			}
			pos = room.FindRoomPos(o, door.NextDoorPos());
		}
		else{
			room = m_oRooms[0];
			pos = Vector3.zero;
		}
		var newRoom = Instantiate<GameObject>(room.gameObject, pos, Quaternion.identity).GetComponent<NewRoom>();
		newRoom.gameObject.SetActive(true);
		if (first)
			PushDoors(newRoom);
		else
			PushDoors(newRoom, o);
		m_realRooms.Add(newRoom);
		m_count++;
	}

	private void PushDoors(NewRoom room){
		var list = new List<Door>();
			if (room.NDoor){
				list.Add(room.NDoor);
			}
			if (room.SDoor){
				// list.Add(room.SDoor);
			}
			if (room.WDoor){
				// list.Add(room.WDoor);
			}
			if (room.EDoor){
				list.Add(room.EDoor);
			}
			list.Shuffle(m_rand);
			foreach(var d in list){
				m_doors.Push(d);
			}	
	}

	private void PushDoors(NewRoom room, Orientation orientation){
		var list = new List<Door>();
		if (room.NDoor){
			if (orientation == Orientation.South){
				room.NDoor.Connected = true;
			}
			else{
				list.Add(room.NDoor);
			}
		}
		if (room.SDoor){
			if (orientation == Orientation.North){
				room.SDoor.Connected = true;
			}
			// else{
			// 	list.Add(room.SDoor);
			// }
		}
		if (room.WDoor){
			if (orientation == Orientation.East){
				room.WDoor.Connected = true;
			}
			// else{
			// 	list.Add(room.WDoor);
			// }
		}
		if (room.EDoor){
			if (orientation == Orientation.West){
				room.EDoor.Connected = true;
			}
			else{
				list.Add(room.EDoor);
			}
		}
		list.Shuffle(m_rand);
		foreach(var d in list){
			m_doors.Push(d);
		}
	}

	private NewRoom FindRoom(Orientation orientation){
		List<NewRoom> list = null;
		switch(orientation){
			case Orientation.South:
				list = m_nRooms;
				break;
			case Orientation.North:
				list = m_sRooms;
				break;
			case Orientation.East:
				list = m_wRooms;
				break;
			case Orientation.West:
				list = m_eRooms;
				break;
		}
		var index = m_rand.Next(list.Count);
		var room = list[index];
		return room;
	}

	public void ParseStr(string roomStr, string name){
		var addDoor = !name.Contains("Start") && !name.Contains("End");
		var obj = new GameObject($"Room_{name}");
		var room = obj.AddComponent<NewRoom>();
		if (!addDoor){
			m_oRooms.Add(room);
		}
		room.WallPrefab = DefaultTile;
		m_currentPos = transform.position;
		foreach(var c in roomStr){
			if (c == '\n'){
				m_currentPos = new Vector3(transform.position.x, 0, m_currentPos.z - 1);
			}
			else if (c == ' '){
				m_currentPos += new Vector3(1, 0, 0);
			}
			else{
				var tile = Instantiate<GameObject>(map[c], m_currentPos, Quaternion.identity, obj.transform);
				m_currentPos += new Vector3(1, 0, 0);
				if (c == 'N'){
					var door = tile.GetComponent<Door>();
					door.Orient(Orientation.North);
					room.NDoor = door;
					if (addDoor)
						m_nRooms.Add(room);
				}
				else if (c == 'S'){
					var door = tile.GetComponent<Door>();
					door.Orient(Orientation.South);
					room.SDoor = door;
					if (addDoor)
						m_sRooms.Add(room);
				}
				else if (c == 'W'){
					var door = tile.GetComponent<Door>();
					door.Orient(Orientation.West);
					room.WDoor = door;
					if (addDoor)
						m_wRooms.Add(room);
				}
				else if (c == 'E'){
					var door = tile.GetComponent<Door>();
					door.Orient(Orientation.East);
					room.EDoor = door;
					if (addDoor)
						m_eRooms.Add(room);
				}
			}
		}
		obj.SetActive(false);
	}
}
