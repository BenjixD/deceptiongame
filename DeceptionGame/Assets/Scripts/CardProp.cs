using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardProp : InteractableObject {
    
    private Card card;

    public void Initialize(Card card) {
        this.card = card;
    }

    protected override void PickUp() {
        Debug.Log("Picked up prop: " + gameObject.name);

        // For now, just assume it goes to the main player:
        GameManager.Instance.controller.mainPlayer.cardController.AddCardToHand(card);

        _interactable = false;
        Destroy(gameObject);
    }
    
    protected override void Repair(PlayerController player) {
        // TODO: leave behind debris
        Debug.Log("Destroyed prop: " + gameObject.name);
        _interactable = false;
        Destroy(gameObject);
    }
    
}
