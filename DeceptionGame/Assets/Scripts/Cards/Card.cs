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

    protected Vector3 GetDirectionHelper() {
        Vector3 direction = Vector3.zero;
        PlayerHorizontalDirection horizontalDirection = this.player.GetHorizontalDirection();
        PlayerVerticalDirection verticalDirection = this.player.GetVerticalDirection();

        if (horizontalDirection != PlayerHorizontalDirection.DEFAULT) {
            direction.x = this.player.GetHorizontalDirection() == PlayerHorizontalDirection.LEFT ? -1 : 1;
        }

        if (verticalDirection != PlayerVerticalDirection.DEFAULT) {
            direction.z = this.player.GetVerticalDirection() == PlayerVerticalDirection.DOWN ? -1 : 1;
        }

        // If neutral, do right
        if (direction.x == 0 && direction.z == 0) {
            direction.x = 1;
        }

        return direction;
    }

}
