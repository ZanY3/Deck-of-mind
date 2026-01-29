using TMPro;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private TMP_Text healthTxt;
    [SerializeField] private TMP_Text damageTxt;


    [Space]
    [Header("Not required")]
    [SerializeField] private TMP_Text debuffDamageTxt;

    [HideInInspector] public bool stunned = false;
    [HideInInspector] public bool strengthWeakened = false;
    [HideInInspector] public bool hpWeakened = false;

    private PlayerHealth player;

    [HideInInspector] public int currentHealth;
    [HideInInspector] public int maxHealth;
    [HideInInspector] public int damage;

    private BattleManager battleManager;
    [HideInInspector] public int stunTurnsLeft = 0;
    [HideInInspector] public int hpWeakenedTurnsLeft = 0;
    [HideInInspector] public int strengthWeakenedTurnsLeft = 0;

    public EnemyData Data => enemyData;


    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        battleManager = FindAnyObjectByType<BattleManager>();

        battleManager.AddEnemy(this);

        ReadData();
        UpdateUI();
    }
//---------------------------------------------------------------------------------------------
    public void ReadData()
    {
        currentHealth = enemyData.health;
        maxHealth = currentHealth;
        damage = enemyData.damage;
    }
    public void UpdateUI()
    {
        healthTxt.text = currentHealth.ToString();
        damageTxt.text = damage.ToString();
        if(debuffDamageTxt != null)
        {
            debuffDamageTxt.text = GetComponent<AnxietyDebuff>().AnxietyDamage.ToString();
        }
    }
    public void TakeDamage(int value) //Maybe it will be overrided in boss to
    {
        transform.DOShakeScale(0.15f, 1, 10, 80);
        currentHealth -= value;
        UpdateUI();
        //Some effects
        if (currentHealth <= 0)
        {
            currentHealth = 0; //for ui
            UpdateUI();

            if (enemyData.name == "Brain Leech" && FindAnyObjectByType<BossPhaseController>() != null)
            {
                FindAnyObjectByType<BossPhaseController>().enemiesSummonedCount--;
            }
            if (GetComponent<Boss>() != null)
            {
                battleManager.EndBtnSetActive(false);
                transform.DOShakeScale(1, 2, 10).SetEase(Ease.Linear).OnComplete(() =>
                {
                    battleManager.RemoveEnemy(this);
                    battleManager.CheckPlayerWin();
                    if(battleManager.enemies.Count > 0)
                    {
                        battleManager.EndBtnSetActive(true);
                    }
                    Destroy(gameObject);
                });
            }
            else
            {
                transform.DOScale(0, 0.15f).SetEase(Ease.Flash).OnComplete(() =>
                {
                    battleManager.RemoveEnemy(this);
                    battleManager.CheckPlayerWin();
                    Destroy(gameObject);
                });
            }
        }
    }
    public void ApplyStun()
    {
        GetComponentInChildren<EnemyToolTip>().UpdateStunToolTip(true);
        stunned = true;
    }
    /*
    public void ApplyHealthWeaken(bool state, int turns)
    {
        hpWeakenedTurnsLeft = turns;
        if (state)
        {
            currentHealth /= 2;
            hpWeakened = true;
        }
        else
        {
            currentHealth *= 2;
            hpWeakened = false;
        }
        UpdateUI();
    }
    public void ApplyStrengthWeaken(bool state, int turns)
    {
        strengthWeakenedTurnsLeft = turns;
        if (state)
        {
            damage /= 2;
            strengthWeakened = true;
        }
        else
        {
            damage *= 2;
            strengthWeakened = false;
        }
        UpdateUI();
    }
    */
    public virtual void AttackPlayer()
    {
        Vector3 tempPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        transform.DOMoveX(player.GetComponent<Transform>().position.x, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            player.TakeDamage(damage);
            transform.DOMoveX(tempPos.x, 0.2f).SetEase(Ease.InOutQuad);
        });
    }
}
