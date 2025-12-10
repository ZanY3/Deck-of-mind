using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private HandManager handManager;

    [Header("UI")]
    [Space]
    [SerializeField] private GameObject endTurnBtn;
    [SerializeField] private GameObject finalPanel;
    [SerializeField] private TMP_Text finalTxt;

    [HideInInspector] public bool isPlayerTurn = true;

    private List<Enemy> enemies;

    private void Awake()
    {
        enemies = new List<Enemy>();
    }

    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }
    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    public void EndPlayerTurn() //When we press "End turn"
    {
        handManager.ClearHand();
        isPlayerTurn = false;
        endTurnBtn.SetActive(false);
        EnemyTurn();
    }
    public void EnemyTurn()
    {
        StartCoroutine(EnemyAttack());
    }
    public void CheckPlayerWin()
    {
        if (enemies.Count <= 0)
        {
            //RoundEnded
            handManager.ClearHand();
            endTurnBtn.SetActive(false);
            finalPanel.SetActive(true);
            finalTxt.text = "Congratulations! You have cleared this round!";
        }
    }
    public void PlayerLose()
    {
        //RoundEnded
        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].enabled = false;
        }
        handManager.ClearHand();
        endTurnBtn.SetActive(false);
        finalPanel.SetActive(true);
        finalTxt.text = "Oh no! You have lost this round!";
    }

    IEnumerator EnemyAttack()
    {
        for (int i = 0; i < enemies.Count; i++)// All enemies attack in turn
        {
            enemies[i].AttackPlayer();
            //Some animations for enemy attack
            yield return new WaitForSeconds(1.5f);
        }
        isPlayerTurn = true;
        endTurnBtn.SetActive(true);
        handManager.DrawHand();
    }
}
