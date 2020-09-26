using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CardPool", order = 0)]
public class CardPool : ScriptableObject
{
    public List<Card> cardsInPool;

    [Header("Debug only variables")]
    public bool tmp_useDebugCard = false;
    public Card tmp_debugCard;

    public Card GetCardFromPool() {
        int randIndex = Random.Range(0, cardsInPool.Count);
        Card card = this.cardsInPool[randIndex];

        if (tmp_useDebugCard) {
            card = tmp_debugCard;
        }

        return Instantiate(card);
    }

    public List<Card> GetCardsFromPool(int count) {
        List<Card> cards = new List<Card>();
        for (int i = 0; i < count; i++) {
            cards.Add(GetCardFromPool());
        }

        return cards;
    }
}
