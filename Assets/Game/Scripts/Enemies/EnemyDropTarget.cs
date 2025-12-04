using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyDropTarget : MonoBehaviour, IDropHandler
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void OnDrop(PointerEventData eventData) //OnCardDrop
    {
        eventData.pointerDrag.GetComponent<CardDrag>().droppedOnTarget = true;
        CardData card = eventData.pointerDrag.GetComponent<CardDisplay>().Card;

        if(card.type == CardData.CardType.Attack)
        {
            enemy.TakeDamage(card.power);
        }
        else
        {
            Debug.Log($"You can't drag '{card.type.ToString()}' card on enemy!");
        }
    }
}
