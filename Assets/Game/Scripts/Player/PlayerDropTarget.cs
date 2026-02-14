using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerDropTarget : MonoBehaviour, IDropHandler
{
    [SerializeField] private CardEffects cardEffects;
    [SerializeField] private EnergyManager energyManager;

    [Space]
    [Header("Sounds")]
    [SerializeField] private AudioClip cleansingSound;
    [Range(0f, 1f)][SerializeField] private float cleansingVolume;

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
                float randPitch = Random.Range(0.9f, 1.1f);
                SoundManager.Instance.PlaySFX(cleansingSound, randPitch, cleansingVolume);

                cardEffects.Cleansing(GetComponent<PlayerHealth>());
                energyManager.DecreaseEnergy(card.energyCost);
                GetComponent<PlayerHealth>().ChangeDraggingClueState(false);
                InteractionState.isDraggingCard = false;
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
