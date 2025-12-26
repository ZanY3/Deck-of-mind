using JetBrains.Annotations;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class DeckManager : MonoBehaviour
{   
    [SerializeField] private List<CardData> startDeck;

    [SerializeField] private List<CardData> deck;
    [SerializeField] private List<CardData> discardPile;

    private void Awake() // Must be initialized in Awake (used by other scripts in Start)
    {
        deck = new List<CardData>(startDeck);
        discardPile = new List<CardData>();
        
    }
    private void Start()
    {
        Shuffle(deck);
    }

    public void Shuffle(List<CardData> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rnd = Random.Range(0, i + 1);
            (deck[i], deck[rnd]) = (deck[rnd], deck[i]);
        }
    }
    public CardData DrawCard()
    {
        if (deck.Count == 0)
        {
            ReshuffleDiscardPile();
        }

        if (deck.Count == 0)
        {
            Debug.LogError("No cards left at all!");
            return null;
        }

        CardData card = deck[^1];
        discardPile.Add(card);
        deck.RemoveAt(deck.Count - 1);
        return card;
    }
    public void ReshuffleDiscardPile()
    {
        deck = new List<CardData>(discardPile);
        discardPile.Clear();
        Shuffle(deck);
    }
}
