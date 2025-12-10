using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerDropTarget : MonoBehaviour, IDropHandler
{
    private PlayerDefense defense;
    private void Start()
    {
        defense = GetComponent<PlayerDefense>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        CardData card = eventData.pointerDrag.GetComponent<CardDisplay>().cardToDisplay;

        if (card.type != CardData.CardType.Defence)
        {
            return;
        }
        else
        {
            defense.AddArmor(card.power);
        }
        eventData.pointerDrag.GetComponent<CardDrag>().droppedOnTarget = true;
    }
}
