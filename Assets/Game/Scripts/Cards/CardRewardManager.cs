using UnityEngine;
using System.Collections.Generic;

public class CardRewardManager : MonoBehaviour
{
    [SerializeField] private List<CardData> allCards;

    private List<CardData> cards;

    [SerializeField] private CardDisplay[] cardTemplates;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private PlayerHealth player;
    [SerializeField] private StageManager stageManager;

    [Header("Sounds")]
    [SerializeField] private AudioClip rewardOpenedSound;
    [SerializeField] private AudioClip healSound;
    [Range(0f, 1f)][SerializeField] private float healVolume;
    [Range(0f, 1f)][SerializeField] private float rewardOpenVolume;

    [HideInInspector] public bool hasChosenCard = false;

    private int cardsCount;

    private void Start()
    {
        // В stage manager музыка меняется сама
        stageManager.PlayAllGameMusic();
    }

    public void GetRewardCards(int count)
    {
        float randPitch = Random.Range(0.85f, 1.05f);
        SoundManager.Instance.PlaySFX(rewardOpenedSound, randPitch, rewardOpenVolume);

        if (cards == null || cards.Count < count)
        {
            cards = new List<CardData>(allCards);
        }

        cardsCount = count;
        SetCardsInteractable(true);

        List<CardData> rewardCards = new List<CardData>();
        List<CardData> availableNewCards = new List<CardData>();

        foreach (var card in allCards)
        {
            if (!deckManager.HasCard(card))
                availableNewCards.Add(card);
        }

        if (availableNewCards.Count > 0)
        {
            int randIndex = Random.Range(0, availableNewCards.Count);
            rewardCards.Add(availableNewCards[randIndex]);
            availableNewCards.RemoveAt(randIndex);
        }

        List<CardData> tempCards = new List<CardData>(allCards);
        tempCards.RemoveAll(c => rewardCards.Contains(c));

        while (rewardCards.Count < count && tempCards.Count > 0)
        {
            int randIndex = Random.Range(0, tempCards.Count);
            rewardCards.Add(tempCards[randIndex]);
            tempCards.RemoveAt(randIndex);
        }

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
        float randPitch = Random.Range(0.85f, 1.05f);
        SoundManager.Instance.PlaySFX(healSound, randPitch, healVolume);

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
