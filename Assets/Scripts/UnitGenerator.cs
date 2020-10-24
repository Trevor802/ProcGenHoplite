using System.Collections.Generic;
using UnityEngine;

public class UnitGenerator : MonoBehaviour {
    public static UnitGenerator Instance {get; private set;}
	public Player PlayerPrefab;
    public Enemy WarriorPrefab;
    public Enemy ArcherPrefab;
    private List<Room> m_rooms;
    private int m_currentRoom = 0;
    private void Awake() {
        Instance = this;
    }
    public void Generate(List<Room> rooms){
        m_rooms = rooms;
        var start = GameObject.FindWithTag("Start");
        var player = Instantiate<GameObject>(PlayerPrefab.gameObject, start.transform.position, Quaternion.identity);
        FillNextRoom();
    }

    public void FillNextRoom(){
        m_rooms[m_currentRoom].GenerateUnits(m_currentRoom+1);
        m_currentRoom++;
    }
}