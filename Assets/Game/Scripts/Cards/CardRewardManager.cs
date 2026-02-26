using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardRewardManager : MonoBehaviour
{
    [SerializeField] private List<CardData> allCards;

    private List<CardData> cards;

    [SerializeField] private Button rerollBtn;
    [SerializeField] private CardDisplay[] cardTemplates;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private PlayerHealth player;
    [SerializeField] private StageManager stageManager;

    [Header("Sounds")]
    [SerializeField] private AudioClip rewardOpenedSound;
    [SerializeField] private AudioClip rerollSound;
    [Range(0f, 1f)][SerializeField] private float rewardOpenVolume;
    [Range(0f, 1f)][SerializeField] private float rerollVolume;

    [HideInInspector] public bool hasChosenCard = false;

    private int cardsCount;

    private void Start()
    {
        stageManager.PlayAllGameMusic();
    }

    public void GetRewardCards(int count)
    {
        float randPitch = Random.Range(0.85f, 1.05f);
        SoundManager.Instance.PlaySFX(rewardOpenedSound, randPitch, rewardOpenVolume);

        cardsCount = count;
        rerollBtn.interactable = true;

        GenerateRewardCards(count);
    }
    private void GenerateRewardCards(int count)
    {
        SetCardsInteractable(true);

        List<CardData> rewardCards = new List<CardData>();

        List<CardData> newCards = allCards.FindAll(c => !deckManager.HasCard(c));
        List<CardData> oldCards = new List<CardData>(allCards);
        oldCards.RemoveAll(c => newCards.Contains(c));

        int newCardsToAdd = Mathf.Min(2, newCards.Count);
        for (int i = 0; i < newCardsToAdd; i++)
        {
            int randIndex = Random.Range(0, newCards.Count);
            rewardCards.Add(newCards[randIndex]);
            newCards.RemoveAt(randIndex);
        }

        List<CardData> pool = new List<CardData>(allCards);
        pool.RemoveAll(c => rewardCards.Contains(c));

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
    public void RerollRewards()
    {
        rerollBtn.interactable = false;

        Sequence seq = DOTween.Sequence();

        // 1. Анимация исчезновения
        for (int i = 0; i < cardsCount; i++)
        {
            CanvasGroup cg = cardTemplates[i].GetComponent<CanvasGroup>();
            Transform t = cardTemplates[i].transform;

            seq.Join(cg.DOFade(0f, 0.2f));
            seq.Join(t.DOScale(0.8f, 0.2f));
        }

        // 2. После исчезновения — меняем карты
        seq.AppendCallback(() =>
        {
            GenerateRewardCards(cardsCount);
        });

        // 3. Анимация появления
        for (int i = 0; i < cardsCount; i++)
        {
            CanvasGroup cg = cardTemplates[i].GetComponent<CanvasGroup>();
            Transform t = cardTemplates[i].transform;

            seq.AppendCallback(() =>
            {
                SoundManager.Instance.PlaySFX(rerollSound, 1, rerollVolume);
            });

            seq.Append(cg.DOFade(1f, 0.25f));
            seq.Join(t.DOScale(1f, 0.25f).SetEase(Ease.OutBack));
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
