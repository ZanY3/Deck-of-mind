using UnityEngine;
using System.Collections.Generic;

public class CardRewardManager : MonoBehaviour
{
    [SerializeField] private List<CardData> allCards;

    private List<CardData> cards;

    [SerializeField] private CardDisplay[] cardTemplates;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private PlayerHealth player;

    [HideInInspector] public bool hasChosenCard = false;
    
    private int cardsCount;

    public void GetRewardCards(int count)
    {
        if (cards == null || cards.Count < count)
        {
            cards = new List<CardData>(allCards);
        }

        cardsCount = count;
        SetCardsInteractable(true);

        List<CardData> tempCards = new List<CardData>(cards);

        for (int i = 0; i < count; i++)
        {
            int randNum = Random.Range(0, tempCards.Count);
            CardData chosenCard = tempCards[randNum];

            cardTemplates[i].cardToDisplay = chosenCard;
            cardTemplates[i].VisualizeCard();

            tempCards.RemoveAt(randNum);
        }
    }
    public void SetCardsInteractable(bool state)
    {
        for (int i = 0; i < cardsCount; i++)
        {
            hasChosenCard = !state;
            CanvasGroup cg = cardTemplates[i].GetComponent<CanvasGroup>();
            cg.interactable = state;
            cg.blocksRaycasts = state;
        }
    }
    public void SkipReward()
    {
        player.Heal(10);
        hasChosenCard = true;
        SetCardsInteractable(false);
    }

    public void ChooseCard(CardData card)
    {
        deckManager.AddCard(card);
        cards.Remove(card);

        SetCardsInteractable(false);
    }
}
