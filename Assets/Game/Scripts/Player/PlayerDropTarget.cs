using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerDropTarget : MonoBehaviour, IDropHandler
{
    [SerializeField] private CardEffects cardEffects;
    [SerializeField] private EnergyManager energyManager;
    private PlayerDefense defense;

    private void Start()
    {
        defense = GetComponent<PlayerDefense>();
    }
//--------------------------------------------------------------------------------------
    public void OnDrop(PointerEventData eventData)
    {
        CardData card = eventData.pointerDrag.GetComponent<CardDisplay>().cardToDisplay;

        if (card.type != CardData.CardType.Defence && card.type != CardData.CardType.SkillOnPlayer)
        {
            return;
        }
        else if (card.type == CardData.CardType.Defence)
        {
            defense.AddArmor(card.power);
            energyManager.DecreaseEnergy(card.energyCost);
        }
        else if (card.type == CardData.CardType.SkillOnPlayer)
        {
            CardData cardTemp = eventData.pointerDrag.GetComponent<CardDisplay>().cardToDisplay;
            if (cardTemp.effect == CardData.Effect.Cleansing && GetComponent<PlayerHealth>().hasAnxiety)
            {
                cardEffects.Cleansing(GetComponent<PlayerHealth>());
                energyManager.DecreaseEnergy(card.energyCost);
                
                Destroy(eventData.pointerDrag);
            }
            if(cardTemp.effect == CardData.Effect.BloodPact)
            {
                cardEffects.BloodPact(GetComponent<PlayerHealth>());
                energyManager.DecreaseEnergy(card.energyCost);
            }
            if(cardTemp.effect == CardData.Effect.BloodDraw)
            {
                cardEffects.BloodDraw(GetComponent<PlayerHealth>());
                energyManager.DecreaseEnergy(card.energyCost);
            }
        }
    }
}
