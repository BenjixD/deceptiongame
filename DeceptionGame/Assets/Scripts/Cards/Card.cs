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

        
    // Called when this is called: this.player.cardController.RemoveCardFromPlayList(this);
    public virtual void OnCardDestroyed() {
    }

    protected Vector3 GetDirectionHelper() {
        Vector3 direction = Vector3.zero;
        PlayerHorizontalDirection horizontalDirection = this.player.mvController.GetHorizontalDirection();
        PlayerVerticalDirection verticalDirection = this.player.mvController.GetVerticalDirection();

        if (horizontalDirection != PlayerHorizontalDirection.DEFAULT) {
            direction.x = this.player.mvController.GetHorizontalDirection() == PlayerHorizontalDirection.LEFT ? -1 : 1;
        }

        if (verticalDirection != PlayerVerticalDirection.DEFAULT) {
            direction.z = this.player.mvController.GetVerticalDirection() == PlayerVerticalDirection.DOWN ? -1 : 1;
        }

        // If neutral, do right
        if (direction.x == 0 && direction.z == 0) {
            direction.x = 1;
        }

        return direction;
    }

    protected void RemoveCardFromActiveList() {
        this.player.cardController.RemoveCardFromPlayList(this);
    }

}
