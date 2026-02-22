using TMPro;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;
    [SerializeField] private TMP_Text healthTxt;
    [SerializeField] private TMP_Text damageTxt;
    public AttackPattern attackPattern;
    public enum AttackPattern
    {
        Normal,
        Scaling,
        Random
    }

    [Space]
    [Header("Not required")]
    [SerializeField] private bool increaseHpEveryTurn = false;
    [Header("For scaling pattern")]
    [SerializeField] private int damageScaleAmout;
    [Header("For random pattern")]
    [SerializeField] private int minRandDamage;
    [SerializeField] private int maxRandDamage;

    [SerializeField] private AudioClip damageChangeSound;
    [SerializeField] private float damageChangeVolume;

    [Space]
    [SerializeField] private TMP_Text debuffDamageTxt;

    [Header("Sounds")]
    [SerializeField] private AudioClip bossDefeatedSound;
    [Range(0f, 1f)][SerializeField] private float bossDefeatedVolume;

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
        GetComponentInChildren<EnemyToolTip>().enemyData = this.enemyData;
        battleManager.AddEnemy(this);
        ReadData();
        if(attackPattern == AttackPattern.Random)
        {
            float randPitch = Random.Range(0.9f, 1.1f);
            SoundManager.Instance.PlaySFX(damageChangeSound, randPitch, damageChangeVolume);
            damage = Random.Range(minRandDamage, maxRandDamage+1);
        }
        UpdateUI();
    }

    public void ReadData()
    {
        currentHealth = enemyData.health;
        maxHealth = currentHealth;
        damage = enemyData.damage;
    }

    public void UpdateUI()
    {
        EnemyToolTip tooltip = GetComponentInChildren<EnemyToolTip>();
        if (healthTxt != null) healthTxt.text = currentHealth.ToString();
        if (damageTxt != null) damageTxt.text = damage.ToString();
        if (debuffDamageTxt != null && GetComponent<AnxietyDebuff>() != null)
        {
            debuffDamageTxt.text = GetComponent<AnxietyDebuff>().anxietyDamage.ToString();
        }
    }

    public void TakeDamage(int value)
    {
        transform.DOKill();

        transform.DOShakeScale(0.15f, 1, 10, 80).SetAutoKill(true).SetUpdate(true);

        currentHealth -= value;
        UpdateUI();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            UpdateUI();

            transform.DOKill();

            if (enemyData.name == "Brain Leech" && FindAnyObjectByType<BossPhaseController>() != null)
            {
                FindAnyObjectByType<BossPhaseController>().enemiesSummonedCount--;
            }

            if (GetComponent<Boss>() != null)
            {
                SoundManager.Instance.PlaySFX(bossDefeatedSound, 1, bossDefeatedVolume);
                battleManager.EndBtnSetActive(false);

                transform.DOShakeScale(1f, 2f, 10)
                    .SetEase(Ease.Linear).SetAutoKill(true)
                    .SetUpdate(true).
                    OnComplete(() =>
                    {
                        if (this != null)
                        {
                            battleManager.RemoveEnemy(this);
                            battleManager.CheckPlayerWin();
                            if (battleManager.enemies.Count > 0)
                                battleManager.EndBtnSetActive(true);
                            transform.DOKill();
                            Destroy(gameObject);
                        }
                    });
            }
            else
            {
                transform.DOScale(0, 0.15f)
                    .SetEase(Ease.Flash).SetAutoKill(true)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        if (this != null)
                        {
                            battleManager.RemoveEnemy(this);
                            battleManager.CheckPlayerWin();
                            transform.DOKill();
                            Destroy(gameObject);
                        }
                    });
            }
        }
    }

    public void ApplyStun()
    {
        var tooltip = GetComponentInChildren<EnemyToolTip>();
        if (tooltip != null) tooltip.UpdateStunToolTip(true);
        stunned = true;
    }

    public virtual void AttackPlayer()
    {
        if (player == null) return;

        transform.DOKill();

        Vector3 startPos = transform.position;
        float duration = 0.2f;

        transform.DOMoveX(player.transform.position.x, duration)
            .SetEase(Ease.OutQuad).SetAutoKill(true)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                player.TakeDamage(damage, true);

                transform.DOMoveX(startPos.x, duration).SetEase(Ease.InOutQuad).SetAutoKill(true).SetUpdate(true);
            });
    }
    public void EnemyEndTurn()
    {
        float randPitch = Random.Range(0.9f, 1.1f);
        SoundManager.Instance.PlaySFX(damageChangeSound, randPitch, damageChangeVolume);
        if (attackPattern == AttackPattern.Random)
        {
            damage = Random.Range(minRandDamage, maxRandDamage + 1);
        }
        else if (attackPattern == AttackPattern.Scaling)
        {
            damage += damageScaleAmout;
        }
        if(increaseHpEveryTurn && enemyData.enemyType == EnemyData.EnemyType.Defender)
        {
            if(GetComponentInChildren<DefenseCell>() != null && GetComponentInChildren<DefenseCell>().defenseIsActive)
            {
                maxHealth += 1;
                currentHealth += 1;
            }
        }
        UpdateUI();
    }

}
