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

        // 1️⃣ Список для награды
        List<CardData> rewardCards = new List<CardData>();
        List<CardData> availableNewCards = new List<CardData>();

        // Находим карты, которых нет у игрока
        foreach (var card in allCards)
        {
            if (!deckManager.HasCard(card))
                availableNewCards.Add(card);
        }

        // 2️⃣ Всегда хотя бы одна новая карта
        if (availableNewCards.Count > 0)
        {
            int randIndex = Random.Range(0, availableNewCards.Count);
            rewardCards.Add(availableNewCards[randIndex]);
            availableNewCards.RemoveAt(randIndex);
        }

        // 3️⃣ Остальные карты случайные из всех
        List<CardData> tempCards = new List<CardData>(allCards);
        tempCards.RemoveAll(c => rewardCards.Contains(c)); // избегаем дубликатов в награде

        while (rewardCards.Count < count && tempCards.Count > 0)
        {
            int randIndex = Random.Range(0, tempCards.Count);
            rewardCards.Add(tempCards[randIndex]);
            tempCards.RemoveAt(randIndex);
        }

        // 4️⃣ Отображение карт
        for (int i = 0; i < count; i++)
        {
            cardTemplates[i].cardToDisplay = rewardCards[i];
            cardTemplates[i].VisualizeCard();
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
