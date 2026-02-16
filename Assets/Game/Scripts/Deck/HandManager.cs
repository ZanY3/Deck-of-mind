using DG.Tweening;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab; //Without pool system yet
    [SerializeField] private Transform handParent;
    [SerializeField] private DeckManager deckManager;

    [Header("Sounds")]
    [SerializeField] private AudioClip handDrawSound;
    [Range(0f, 1f)][SerializeField] private float handDrawVolume = 0.075f;

    private int handSize = 3;

    public void DrawHand()
    {
        Random.Range(0, handSize);
        float randPitch = Random.Range(0.9f, 1.1f);
        SoundManager.Instance.PlaySFX(handDrawSound, randPitch, handDrawVolume);

        ClearHand();
        for (int i = 0; i < handSize; i++)
        {
            CardData data = deckManager.DrawCard();
            SpawnCard(data);
            //CardData data = cards[Random.Range(0, cards.Count)];
        }
    }
    public void DrawOneCard()
    {
        CardData data = deckManager.DrawCard();
        SpawnCard(data);
    }
    public int HandCount()
    {
        return handParent.childCount;
    }
    public void SpawnCard(CardData data)
    {
        GameObject card = Instantiate(cardPrefab, handParent);

        card.transform.localScale = Vector3.zero;
        card.transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack);

        card.GetComponent<CardDisplay>().cardToDisplay = data;
        card.GetComponent<CardDisplay>().VisualizeCard();
    }
    public void ClearHand()
    {
        foreach (Transform child in handParent)
            Destroy(child.gameObject);
    }
}
