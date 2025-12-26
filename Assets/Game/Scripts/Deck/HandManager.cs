using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab; //Without pool system yet
    [SerializeField] private List<CardData> cards;
    [SerializeField] private Transform handParent;
    [SerializeField] private DeckManager deckManager;

    private int handSize = 3;

    private void Start()
    {
        DrawHand();
    }

    public void DrawHand()
    {
        ClearHand();
        for (int i = 0; i < handSize; i++)
        {
            CardData data = deckManager.DrawCard();
            SpawnCard(data);
            //CardData data = cards[Random.Range(0, cards.Count)];
        }
    }
    public void SpawnCard(CardData data)
    {
        GameObject card = Instantiate(cardPrefab, handParent);
        card.GetComponent<CardDisplay>().cardToDisplay = data;
        card.GetComponent<CardDisplay>().VisualizeCard();
    }
    public void ClearHand()
    {
        foreach (Transform child in handParent)
            Destroy(child.gameObject);
    }
}
