using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {
    public static UnitManager Instance {get; private set;} = null;
    private List<Unit> m_units = new List<Unit>();
    public bool IsPlayerTurn => m_isPlayerTurn;
    private bool m_isPlayerTurn = true;
    private int m_count;
    private Player m_player;
    private Player player{
        get{
            if (m_player is null){
                m_player = FindObjectOfType<Player>();
            }
            return m_player;
        }
    }
    private void Awake() {
        Instance = this;
    }
    public void RegisterUnit(Unit unit){
        m_units.Add(unit);
    }
    public void DeregisterUnit(Unit unit){
        m_units.Remove(unit);
    }

    public void DestroyAllUnits(){
        for(int i = m_units.Count - 1; i >=0; i--){
            Destroy(m_units[i].gameObject);
        }
        m_units.Clear();
    }

    public void Callback(){
        m_count++;
        if (m_count == m_units.Count){
            AfterCallback();
        }
    }
    private void AfterCallback(){
        m_count = 0;
        m_isPlayerTurn = true;
    }
    public void AfterPlayerTurn(){
        if (m_units.Count == 0){
            return;
        }
        m_isPlayerTurn = false;
        m_count = 0;
        foreach(var u in m_units){
            u.TakeAction(Callback);
        };
    }
}