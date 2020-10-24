using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Unit
{
    public static Player Instance {get; private set;}
    private int m_maxMana = 5;
    private int m_maxHealth = 3;
    private int m_maxJump = 2;
    private int m_mana;
    protected override void Awake() {
        base.Awake();
        Instance = this;
        Restore();
    }
    private void Start() {
        MouseManager.Instance.OnClickTile += this.OnClickTile;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)){
            Restore();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)){
            UnitManager.Instance.DestroyAllUnits();
        }
    }
    internal override void TakeAction(Action callback)
    {
    }
    public void RestoreHP(){
        m_health = m_maxHealth;
        UpdateUI();
    }
    public void IncreaseMaxHP(){
        m_maxHealth++;
    }
    public void IncreaseMaxMana(){
        m_maxMana++;
    }
    public void IncreaseMaxJump(){
        m_maxJump++;
    }
    private void Restore(){
        m_health = m_maxHealth;
        m_mana = m_maxMana;
        UpdateUI();
    }
    private void UpdateUI(){
        UnitGenerator.Instance.HealthText.text = $"Health: {m_health}";
        UnitGenerator.Instance.ManaText.text = $"Mana: {m_mana}";
    }

    public override void Damage(int value)
    {
        base.Damage(value);
        UnitGenerator.Instance.HealthText.text = $"Health: {m_health}";
    }

    private void UseMana(int value){
        m_mana -= value;
        if (m_mana < 0){
            m_mana = 0;
        }
        UnitGenerator.Instance.ManaText.text = $"Mana: {m_mana}";
    }

    private bool CanMove(Tile tile){
        var dir = tile.transform.position - transform.position;
        var dist = dir.magnitude;
        if (dir.IsDiag()){
            return false;
        }
        if (tile == GetTile()){
            return false;
        }
        if (dist >= m_maxJump + 1){
            return false;
        }
        if (Util.NrE(m_maxJump, dist) && m_mana < 2){
            return false;
        }
        return true;
    }

    private bool CanAttack(Tile tile){
        var dir = tile.transform.position - transform.position;
        if (dir.IsDiag()){
            return false;
        }
        if (!CanMove(tile)){
            return false;
        }
        var unit = tile.GetUnit();
        if (unit != null && unit is Enemy){
            return true;
        }
        return false;
    }

    private bool CanPush(Tile tile){
        var dir = tile.transform.position - transform.position;
        if (dir.IsDiag()){
            return false;
        }
        var unit = tile.GetUnit();
        if (unit != null && 
            unit is Enemy &&
            m_mana >= DEF.PSH_MANA){
            return true;
        }
        return false;
    }
    protected override void OnEnterTile(Tile tile){
        base.OnEnterTile(tile);
        if (tile.tag == "Door" && tile.Used == false){
            var door = tile.GetComponentInParent<Door>();
            if (door.IsExit){
                UnitManager.Instance.DestroyAllUnits();
                tile.Used = true;
            }
            else{
                UnitGenerator.Instance.FillNextRoom();
                Restore();
                tile.Used = true;
            }
        }
        else if (tile.tag == "Upgrade" && !tile.Used){
            UnitManager.Instance.ShowButtons();
            tile.Used = true;
        }
    }
    public void OnClickTile(Tile tile){
        if (UnitManager.Instance.IsPlayerTurn){
            if (CanPush(tile)){
                var dir = tile.transform.position - transform.position;
                UseMana(3);
                StartCoroutine(Push(tile, dir, UnitManager.Instance.AfterPlayerTurn));
            }
            else if (CanAttack(tile)){
                StartCoroutine(Attack(tile, UnitManager.Instance.AfterPlayerTurn));
            }
            else if (CanMove(tile)){
                if ((tile.transform.position - transform.position).magnitude >= m_maxJump)
                    UseMana(2);
                StartCoroutine(
                    Move(transform, transform.position, tile.transform.position, MoveSpeed, UnitManager.Instance.AfterPlayerTurn)
                );
            }
        }
    }

    protected override void Die()
    {
        UnitManager.Instance.ShowRestart(false);
    }
}
