using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private HandManager handManager;
    [SerializeField] private EnergyManager energyManager;
    [SerializeField] private CardEffects effects;
    [SerializeField] private PlayerHealth player;

    [Header("UI")]
    [Space]
    [SerializeField] private GameObject endTurnBtn;
    [SerializeField] private GameObject handPanel;

    [SerializeField] private GameObject winFinalPanel;
    [SerializeField] private GameObject loseFinalPanel;

    [HideInInspector] public bool isPlayerTurn = true;

    [HideInInspector] public List<Enemy> enemies;

    private void Awake()
    {
        enemies = new List<Enemy>();
    }
//-------------------------------------------
    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }
    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }
    public void StartBattle()
    {
        isPlayerTurn = true;

        EndBtnSetActive(true);

        RectTransform rt = endTurnBtn.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -200);
        EndBtnSetActive(true);

        winFinalPanel.SetActive(false);
        loseFinalPanel.SetActive(false);

        energyManager.RefillEnergy();
    }
    public void EndPlayerTurn() //When we press "End turn"
    {
        energyManager.RefillEnergy();
        handManager.ClearHand();
        isPlayerTurn = false;

        EndBtnSetActive(false);
        EnemyTurn();
    }
    public void EnemyTurn()
    {
        if(player.hasAnxiety)
        {
            Debug.LogWarning("Player took damage from anxiety debuff");
            player.TakeDamage(player.anxietyDamage,true);
        }
        StartCoroutine(EnemyAttack());
    }
    public void CheckPlayerWin()
    {
        if (enemies.Count <= 0)
        {
            //RoundEnded
            player.hasAnxiety = false;
            player.anxietyDamage = 0;

            player.UpdateUI();

            handManager.ClearHand();
            EndBtnSetActive(false);
            winFinalPanel.SetActive(true);
        }
    }
    public void PlayerLose()
    {
        //RoundEnded
        player.hasAnxiety = false;
        player.anxietyDamage = 0;

        player.UpdateUI();

        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].enabled = false;
        }
        EndBtnSetActive(false);
        loseFinalPanel.SetActive(true);
        handPanel.SetActive(false);
    }

    IEnumerator EnemyAttack()
    {
        if(player.hasAnxiety)
        {
            yield return new WaitForSeconds(1.5f);
        }
        for (int i = 0; i < enemies.Count; i++) // All enemies attack in turn
        {
            if (enemies[i].Data.enemyType == EnemyData.EnemyType.Defender && enemies[i].GetComponentInChildren<DefenseCell>() != null && enemies[i].stunned == false)
            {
                if (enemies[i].GetComponentInChildren<DefenseCell>().RefreshDefenseEveryTurn)
                {
                    enemies[i].transform.DOShakeScale(duration: 0.15f, strength: new Vector3(0.15f, 0.15f, 0));
                    enemies[i].GetComponentInChildren<DefenseCell>().RefillDefense();
                    yield return new WaitForSeconds(1.5f);
                }
            }

            if(enemies[i].Data.enemyType == EnemyData.EnemyType.Debuffer)
            {
                if (enemies[i].GetComponent<AnxietyDebuff>() != null)
                {
                    if (!player.hasAnxiety && !enemies[i].GetComponent<AnxietyDebuff>().startAnxietyApplied && enemies[i].stunned == false)
                    {
                        Enemy enemy = enemies[i];

                        Tween castTween = enemy.transform.DOShakePosition(0.25f, new Vector3(6f, 3f, 0), 12, 90, false, true);
                        enemy.GetComponent<AnxietyDebuff>().startAnxietyApplied = true;
                        player.hasAnxiety = true;
                        player.anxietyDamage = enemy.GetComponent<AnxietyDebuff>().AnxietyDamage;
                        player.UpdateUI();

                        yield return castTween.WaitForCompletion(); //ЖДЁМ АНИМАЦИЮ

                    }
                }
                else if (enemies[i].GetComponent<StunDebuff>() != null && enemies[i].stunned == false)
                {
                    StunDebuff enemyStun = enemies[i].GetComponent<StunDebuff>();
                    EnemyToolTip enemyToolTip = enemies[i].GetComponentInChildren<EnemyToolTip>();

                    if(enemyStun.turnsUntilStun > 0)
                    {
                        enemyStun.turnsUntilStun--;
                    }
                    if (enemyStun.turnsUntilStun <= 0)
                    {
                        enemyStun.DealStun();

                        enemyToolTip.UpdateStunClue(false);
                    }
                    if (enemyStun.turnsUntilStun == 1)
                    {
                        enemyToolTip.UpdateStunClue(true);
                    }
                 }
            }

            if (enemies[i].stunned)
            {
                enemies[i].stunTurnsLeft--;
                if (enemies[i].stunTurnsLeft <= 0)
                {
                    enemies[i].GetComponentInChildren<EnemyToolTip>().UpdateStunToolTip(false);
                    enemies[i].stunned = false;
                }
                continue;
            }
            if (enemies[i].hpWeakened)
            {
                enemies[i].hpWeakenedTurnsLeft--;
                if (enemies[i].hpWeakenedTurnsLeft <= 0)
                {
                    effects.HealthWeaken(enemies[i], false, 0);
                }
            }
            if (enemies[i].strengthWeakened)
            {
                enemies[i].strengthWeakenedTurnsLeft--;
                if (enemies[i].strengthWeakenedTurnsLeft <= 0)
                {
                    effects.StrengthWeaken(enemies[i], false, 0);
                }
            }
            enemies[i].AttackPlayer();
            //Some animations for enemy attack
            yield return new WaitForSeconds(1.5f);
        }
        if (player.stunned == false)
        {
            player.ChangeStunClueState(false);
            isPlayerTurn = true;
            EndBtnSetActive(true);
            handManager.DrawHand();
        }
        else if (player.stunned == true)
        {
            isPlayerTurn = false;
            EndBtnSetActive(false);
            player.ChangeStunClueState(true);
            if (player.turnsUntilStunRemove <= 0)
            {
                player.ChangeStunState(false);
            }
            else
            {
                player.turnsUntilStunRemove--;
            }
        }
        if (!player.stunned)
        {
            isPlayerTurn = true;
            EndBtnSetActive(true);
            handManager.DrawHand();
        }
        if (winFinalPanel.activeSelf || loseFinalPanel.activeSelf)
            yield break;
    }
    public void EndBtnSetActive(bool state)
    {
        RectTransform endBtnTransform = endTurnBtn.GetComponent<RectTransform>();
        endBtnTransform.DOKill();

        if (!state)
        {
            endBtnTransform.DOAnchorPosY(-200, 0.3f).OnComplete(() =>
            {
                endTurnBtn.SetActive(false);
            });
        }
        else
        {
            endTurnBtn.SetActive(true);
            endBtnTransform.DOAnchorPosY(135, 0.3f);
        }
    }
}
