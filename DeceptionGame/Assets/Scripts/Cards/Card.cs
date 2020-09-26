using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Card", order = 1)]
public class Card : ScriptableObject {

    public string cardName = "CardName";
    [TextArea]
    public string cardDescription = "Card text";

    public bool removeFromHandOnPlay = true;

    protected PlayerController player;

    public void Initialize(PlayerController player) {
        this.player = player;
    }

    public void PlayCard() {
        if (this.removeFromHandOnPlay) {
            this.player.cardController.RemoveCardFromHand(this);
        }

        this.player.cardController.AddCardToPlayList(this);
        this.PlayCardStart();
    }

    protected virtual void PlayCardStart() {
    }

    public virtual void PlayCardUpdate() {
        // Override me!
    }

    public void DestroyMe() {
        this.player.cardController.RemoveCardFromPlayList(this);
    }
}
