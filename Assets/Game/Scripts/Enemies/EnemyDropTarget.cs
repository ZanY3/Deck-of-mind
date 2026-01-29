using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class EnemyDropTarget : MonoBehaviour, IDropHandler
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }
//--------------------------------------------------------------------------------------
    public void OnDrop(PointerEventData eventData) //OnCardDrop
    {
        CardData card = eventData.pointerDrag.GetComponent<CardDisplay>().cardToDisplay;

        if (card.type != CardData.CardType.Attack && card.type != CardData.CardType.SkillOnEnemy)
        {
            return;
        }
        if(card.type == CardData.CardType.Attack)
        {
            if(card.effect == CardData.Effect.RandomPower)
            {
                enemy.TakeDamage(eventData.pointerDrag.GetComponent<CardEffects>().RandomPower());
            }
            else
            {
                enemy.TakeDamage(card.power);
            }
        }
        else if(card.type == CardData.CardType.SkillOnEnemy)
        {
            if(card.effect == CardData.Effect.Stun)
            {
                enemy.transform.DOShakeScale(duration: 0.15f, strength: new Vector3(0.15f, 0.15f, 0));

                eventData.pointerDrag.GetComponent<CardEffects>().Stun(enemy);
            }
            if(card.effect == CardData.Effect.HealthDrain)
            {
                eventData.pointerDrag.GetComponent<CardEffects>().HealthWeaken(enemy, true, 2);
            }
            if(card.effect == CardData.Effect.StrengthDrain)
            {
                eventData.pointerDrag.GetComponent<CardEffects>().StrengthWeaken(enemy, true, 2);
            }
        }
        eventData.pointerDrag.GetComponent<CardDrag>().droppedOnTarget = true;
    }
}
