using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.VisualScripting;

public class EnemyDropTarget : MonoBehaviour
{
    [Header("Not required")]
    [SerializeField] private GameObject skillCanceledClue;
    [SerializeField] private bool immuneToDebuffs;
    [SerializeField] private AudioClip deniedSound;
    [SerializeField] private float deniedVolume;

    [HideInInspector] public bool canBeAttacked = true;
    private Enemy enemy;
    private CardEffects effects;
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        effects = FindAnyObjectByType<CardEffects>();
    }

    public void ApplyAttack(CardData card)
    {
        if (card.type == CardData.CardType.Attack)
        {
            if(card.effect == CardData.Effect.ShieldSlam)
            {
                enemy.TakeDamage(effects.ShieldSlam());
            }
            else if(card.effect == CardData.Effect.StrikeTheHelpless)
            {
                enemy.TakeDamage(effects.StrikeTheHelpless(enemy));
            }
            else
            {
                int dmg = card.effect == CardData.Effect.RandomPower ? effects.RandomPower() : card.power;
                enemy.TakeDamage(dmg);
            }
        }
        else if (card.type == CardData.CardType.SkillOnEnemy)
        {
            if(immuneToDebuffs == true)
            {
                SoundManager.Instance.PlaySFX(deniedSound, Random.Range(0.95f, 1.05f), deniedVolume);
                skillCanceledClue.SetActive(true);

                CanvasGroup cg = skillCanceledClue.GetComponent<CanvasGroup>();
                if (cg == null)
                    cg = skillCanceledClue.AddComponent<CanvasGroup>();

                cg.alpha = 1f;

                // Через 2 секунды плавно исчезает
                cg.DOFade(0f, 0.5f)
                  .SetDelay(1f)
                  .OnComplete(() =>
                  {
                      skillCanceledClue.SetActive(false);
                      cg.alpha = 1f; // сброс альфы для следующего показа
                  });
            }
            else
            {
                switch (card.effect)
                {
                    case CardData.Effect.Stun:
                        if (!enemy.stunned)
                        {
                            enemy.transform.DOShakeScale(0.15f, new Vector3(0.15f, 0.15f, 0)).SetAutoKill(true).SetUpdate(true);
                            effects.Stun(enemy);
                        }
                        break;

                    case CardData.Effect.HealthDrain:
                        if (!enemy.hpWeakened && enemy.currentHealth != 1 && enemy.enemyData.enemyType != EnemyData.EnemyType.Boss)
                        {
                            enemy.transform.DOShakeScale(0.15f, new Vector3(0.15f, 0.15f, 0)).SetAutoKill(true).SetUpdate(true);
                            effects.HealthWeaken(enemy, true, 2);
                        }
                        break;

                    case CardData.Effect.StrengthDrain:
                        if (!enemy.strengthWeakened && enemy.damage != 1)
                        {
                            enemy.transform.DOShakeScale(0.15f, new Vector3(0.15f, 0.15f, 0)).SetAutoKill(true).SetUpdate(true);
                            effects.StrengthWeaken(enemy, true, 2);
                        }
                        break;
                }
            }
        }
    }
}
