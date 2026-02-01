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

    public void ReadData()
    {
        currentHealth = enemyData.health;
        maxHealth = currentHealth;
        damage = enemyData.damage;
    }

    public void UpdateUI()
    {
        if (healthTxt != null) healthTxt.text = currentHealth.ToString();
        if (damageTxt != null) damageTxt.text = damage.ToString();
        if (debuffDamageTxt != null && GetComponent<AnxietyDebuff>() != null)
        {
            debuffDamageTxt.text = GetComponent<AnxietyDebuff>().AnxietyDamage.ToString();
        }
    }

    public void TakeDamage(int value)
    {
        // Убиваем все tween на transform перед новой анимацией
        transform.DOKill();

        // Анимация удара
        transform.DOShakeScale(0.15f, 1, 10, 80).SetAutoKill(true).SetUpdate(true);

        currentHealth -= value;
        UpdateUI();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            UpdateUI();

            // Отключаем все текущие tween на этом объекте
            transform.DOKill();

            if (enemyData.name == "Brain Leech" && FindAnyObjectByType<BossPhaseController>() != null)
            {
                FindAnyObjectByType<BossPhaseController>().enemiesSummonedCount--;
            }

            if (GetComponent<Boss>() != null)
            {
                battleManager.EndBtnSetActive(false);

                transform.DOShakeScale(1f, 2f, 10)
                    .SetEase(Ease.Linear).SetAutoKill(true)
                    .SetUpdate(true).
                    OnComplete(() =>
                    {
                        if (this != null) // Проверка на объект
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

        // Убиваем все tween на transform, чтобы избежать конфликтов
        transform.DOKill();

        Vector3 startPos = transform.position;
        float duration = 0.2f;

        transform.DOMoveX(player.transform.position.x, duration)
            .SetEase(Ease.OutQuad).SetAutoKill(true)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                player.TakeDamage(damage, true);

                // Возврат в начальную позицию
                transform.DOMoveX(startPos.x, duration).SetEase(Ease.InOutQuad).SetAutoKill(true).SetUpdate(true);
            });
    }
}
