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
            cards = new List<CardData>(allCards);

        cardsCount = count;
        SetCardsInteractable(true);

        List<CardData> rewardCards = new List<CardData>();

        // Получаем список новых карт (которых нет в колоде)
        List<CardData> newCards = allCards.FindAll(c => !deckManager.HasCard(c));
        List<CardData> oldCards = new List<CardData>(allCards);
        oldCards.RemoveAll(c => newCards.Contains(c)); // карты которые уже есть

        // Берем максимум 2 новые карты
        int newCardsToAdd = Mathf.Min(2, newCards.Count);
        for (int i = 0; i < newCardsToAdd; i++)
        {
            int randIndex = Random.Range(0, newCards.Count);
            rewardCards.Add(newCards[randIndex]);
            newCards.RemoveAt(randIndex);
        }

        // Создаем единый pool для всех оставшихся карт (новых и старых), исключая уже выбранные
        List<CardData> pool = new List<CardData>(allCards);
        pool.RemoveAll(c => rewardCards.Contains(c));

        // Заполняем оставшиеся слоты
        while (rewardCards.Count < count && pool.Count > 0)
        {
            int randIndex = Random.Range(0, pool.Count);
            rewardCards.Add(pool[randIndex]);
            pool.RemoveAt(randIndex);
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
