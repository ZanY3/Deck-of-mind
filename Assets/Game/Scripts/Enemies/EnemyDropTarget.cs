using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.VisualScripting;

public class EnemyDropTarget : MonoBehaviour
{
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
            int dmg = card.effect == CardData.Effect.RandomPower ? effects.RandomPower() : card.power;
            enemy.TakeDamage(dmg);
        }
        else if (card.type == CardData.CardType.SkillOnEnemy)
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
                    if (!enemy.hpWeakened && enemy.currentHealth != 1)
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
