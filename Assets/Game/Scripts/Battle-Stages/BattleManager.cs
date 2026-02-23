using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

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

    [Space]
    [Header("Sounds")]
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip defeatSound;
    [SerializeField] private AudioClip endTurnSound;
    [SerializeField] private AudioClip debuffSound;
    [Range(0f, 1f)][SerializeField] private float winVolume;
    [Range(0f, 1f)][SerializeField] private float defeatVolume;
    [Range(0f, 1f)][SerializeField] private float endTurnVolume;
    [Range(0f, 1f)][SerializeField] private float debuffVolume;

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

        winFinalPanel.SetActive(false);
        loseFinalPanel.SetActive(false);

        energyManager.RefillEnergy();
    }
    public void EndPlayerTurn() //When we press "End turn"
    {
        float randPitch = Random.Range(0.9f, 1.1f);
        SoundManager.Instance.PlaySFX(endTurnSound, randPitch, winVolume);

        energyManager.RefillEnergy();
        handManager.ClearHand();
        isPlayerTurn = false;

        EndBtnSetActive(false);
        EnemyTurn();
    }
    public void EnemyTurn()
    {
        // если игрок уже проиграл — враги не ходят
        if (loseFinalPanel.activeSelf)
            return;

        if (player.hasAnxiety)
        {
            Debug.LogWarning("Player took damage from anxiety debuff");
            player.TakeDamage(player.anxietyDamage, true);
        }

        // после урона от тревоги игрок мог умереть
        if (loseFinalPanel.activeSelf)
            return;

        StartCoroutine(EnemyAttack());
    }
    public void CheckPlayerWin()
    {
        if (enemies.Count <= 0)
        {
            //RoundEnded
            float randPitch = Random.Range(0.95f, 1.05f);
            SoundManager.Instance.PlaySFX(winSound, randPitch, winVolume);

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
        SoundManager.Instance.PlaySFX(defeatSound, 1, defeatVolume);
        player.hasAnxiety = false;
        player.anxietyDamage = 0;

        player.UpdateUI();

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].enabled = false;
        }
        EndBtnSetActive(false);
        loseFinalPanel.SetActive(true);
        handPanel.SetActive(false);
    }

    IEnumerator EnemyAttack()
    {
        Debug.Log("=== ENEMY TURN START === Player stunned: " + player.stunned + ", turns left: " + player.turnsUntilStunRemove);

        if (player.hasAnxiety)
        {
            yield return new WaitForSeconds(1.5f);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].Data.enemyType == EnemyData.EnemyType.Defender &&
                enemies[i].GetComponentInChildren<DefenseCell>() != null &&
                enemies[i].stunned == false)
            {
                if (enemies[i].GetComponentInChildren<DefenseCell>().RefreshDefenseEveryTurn)
                {
                    enemies[i].transform.DOShakeScale(0.15f, new Vector3(0.15f, 0.15f, 0));
                    enemies[i].GetComponentInChildren<DefenseCell>().RefillDefense();
                    yield return new WaitForSeconds(1.5f);
                }
            }

            if (enemies[i].Data.enemyType == EnemyData.EnemyType.Debuffer)
            {
                if (enemies[i].GetComponent<AnxietyDebuff>() != null)
                {
                    if (!player.hasAnxiety &&
                        !enemies[i].GetComponent<AnxietyDebuff>().startAnxietyApplied &&
                        enemies[i].stunned == false)
                    {
                        float randPitch = Random.Range(0.9f, 1.1f);
                        SoundManager.Instance.PlaySFX(debuffSound, randPitch, debuffVolume);

                        Enemy enemy = enemies[i];

                        Tween castTween = enemy.transform.DOShakePosition(
                            0.25f,
                            new Vector3(6f, 3f, 0),
                            12, 90, false, true);

                        enemy.GetComponent<AnxietyDebuff>().startAnxietyApplied = true;
                        player.hasAnxiety = true;
                        player.anxietyDamage =
                            enemy.GetComponent<AnxietyDebuff>().anxietyDamage;
                        player.UpdateUI();

                        yield return castTween.WaitForCompletion();
                    }
                }
                else if (enemies[i].GetComponent<StunDebuff>() != null &&
                         enemies[i].stunned == false)
                {
                    StunDebuff enemyStun = enemies[i].GetComponent<StunDebuff>();
                    EnemyToolTip enemyToolTip =
                        enemies[i].GetComponentInChildren<EnemyToolTip>();

                    if (enemyStun.turnsUntilStun > 0)
                    {
                        enemyStun.turnsUntilStun--;
                    }

                    if (enemyStun.turnsUntilStun <= 0)
                    {
                        float randPitch = Random.Range(0.9f, 1.1f);
                        SoundManager.Instance.PlaySFX(debuffSound, randPitch, debuffVolume);

                        enemyStun.DealStun();
                        enemyToolTip.UpdateStunClue(false);
                    }

                    if (enemyStun.turnsUntilStun == 1)
                    {
                        enemyToolTip.UpdateStunClue(true);
                    }

                    yield return new WaitForSeconds(1.5f);
                }
                else if (!enemies[i].stunned && enemies[i].GetComponent<BurnoutDebuff>() != null)
                {
                    BurnoutDebuff enemyBurnout = enemies[i].GetComponent<BurnoutDebuff>();
                    EnemyToolTip enemyToolTip = enemies[i].GetComponentInChildren<EnemyToolTip>();

                    if (enemyBurnout.turnsUntilDebuff > 0)
                    {
                        enemyBurnout.turnsUntilDebuff--;
                    }

                    if (enemyBurnout.turnsUntilDebuff <= 0)
                    {
                        float randPitch = Random.Range(0.9f, 1.1f);
                        SoundManager.Instance.PlaySFX(debuffSound, randPitch, debuffVolume);

                        enemyBurnout.DealBurnout();
                        enemyToolTip.UpdateBurnoutClue(false);
                    }

                    if (enemyBurnout.turnsUntilDebuff == 1)
                    {
                        enemyToolTip.UpdateBurnoutClue(true);
                    }

                    yield return new WaitForSeconds(1.5f);
                }
                else if(!enemies[i].stunned && enemies[i].GetComponent<FearDebuff>() != null)
                {
                    if (!player.hasFear &&!enemies[i].GetComponent<FearDebuff>().fearApplied)
                    {
                        Enemy enemy = enemies[i];
                        Tween castTween = enemy.transform.DOShakePosition(
                            0.25f,
                            new Vector3(6f, 3f, 0),
                            12, 90, false, true);

                        enemy.GetComponent<FearDebuff>().fearApplied = true;
                        player.hasFear = true;
                        player.UpdateUI();
                        yield return castTween.WaitForCompletion();
                    }
                }
            }

            if (enemies[i].stunned)
            {
                enemies[i].stunTurnsLeft--;

                if (enemies[i].stunTurnsLeft <= 0)
                {
                    enemies[i].GetComponentInChildren<EnemyToolTip>()
                        .UpdateStunToolTip(false);
                    enemies[i].stunned = false;
                }

                continue;
            }

            // =============================
            // АТАКА
            // =============================

            enemies[i].AttackPlayer();
            yield return new WaitForSeconds(1.5f);

            // =============================
            // ДЕБАФФЫ СПАДАЮТ ТЕПЕРЬ ПОСЛЕ АТАКИ
            // =============================

            if (enemies[i].hpWeakened)
            {
                enemies[i].hpWeakenedTurnsLeft--;

                enemies[i].GetComponentInChildren<EnemyToolTip>()
                    .UpdateHpWeaknededTooltip(true,
                        enemies[i].hpWeakenedTurnsLeft);

                if (enemies[i].hpWeakenedTurnsLeft <= 0)
                {
                    effects.HealthWeaken(enemies[i], false, 0);
                }
            }

            if (enemies[i].strengthWeakened)
            {
                enemies[i].strengthWeakenedTurnsLeft--;

                enemies[i].GetComponentInChildren<EnemyToolTip>()
                    .UpdateStrengthTooltip(true,
                        enemies[i].strengthWeakenedTurnsLeft);

                if (enemies[i].strengthWeakenedTurnsLeft <= 0)
                {
                    effects.StrengthWeaken(enemies[i], false, 0);
                }
            }
            enemies[i].EnemyEndTurn();
        }

        Debug.Log("=== After enemy attacks === Player stunned: " +
                  player.stunned + ", turns: " +
                  player.turnsUntilStunRemove);

        if (player.stunned)
        {
            player.ChangeStunClueState(true);

            if (player.turnsUntilStunRemove > 0)
            {
                player.turnsUntilStunRemove--;
            }

            if (player.turnsUntilStunRemove <= 0)
            {
                player.ChangeStunState(false);
                player.ChangeStunClueState(false);

                isPlayerTurn = true;
                EndBtnSetActive(true);
                handManager.DrawHand();
            }
            else
            {
                isPlayerTurn = false;
                EndBtnSetActive(false);
                StartCoroutine(EnemyAttack());
            }
        }
        else
        {
            player.ChangeStunClueState(false);
            isPlayerTurn = true;
            EndBtnSetActive(true);
            handManager.DrawHand();
        }

        if (winFinalPanel.activeSelf || loseFinalPanel.activeSelf)
            yield break;
    }

    public void EndBtnSetActive(bool state)
    {
        Button btn = endTurnBtn.GetComponent<Button>();
        Image img = endTurnBtn.GetComponent<Image>();

        img.DOKill();

        btn.interactable = state;

        if (!state)
        {
            img.DOColor(new Color(0.6f, 0.6f, 0.6f, 0.7f), 0.15f)
               .SetEase(Ease.OutQuad);
        }
        else
        {
            img.DOColor(Color.white, 0.15f)
               .SetEase(Ease.OutQuad);
        }
    }
}