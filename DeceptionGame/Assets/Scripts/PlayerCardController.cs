using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardController : MonoBehaviour
{
    public int maxNumCardsInHand = 5;

    private List<Card> activeCardsList = new List<Card>();

    private List<Card> hand = new List<Card>();

    private PlayerController playerController;

    public void Initialize(PlayerController controller) {
        this.playerController = controller;
    }

    public void PlayCardInHand(int cardIndex) {
        if (cardIndex >= hand.Count) {
            return;
        }

        this.hand[cardIndex].PlayCard();
    }

    public void UpdateActiveCards() {
        foreach (Card card in this.activeCardsList) {
            card.PlayCardUpdate();
        }
    }

    public Card DrawRandomCard() {
        return GameManager.Instance.models.cardPool.GetCardFromPool();
    }

    public bool AddCardToHand(Card card) {
        if (this.hand.Count == maxNumCardsInHand) {
            return false;
        } else {
            card.Initialize(this.playerController);
            this.hand.Add(card);
            return true;
        }
    }

    public void RemoveCardFromHand(Card card) {
        this.hand.Remove(card);
    }

    public List<Card> GetActiveCardsList() {
        return activeCardsList;
    }
    
    public List<Card> GetCardsInHand() {
        return this.hand;
    }

    public void AddCardToPlayList(Card card) {
        this.activeCardsList.Add(card);
    }

    public void RemoveCardFromPlayList(Card card) {
        this.activeCardsList.Remove(card);
    }
}
