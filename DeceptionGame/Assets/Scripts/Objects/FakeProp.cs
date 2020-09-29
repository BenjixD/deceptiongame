using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeProp : InteractableObject {

    private TransformCard transformCardController;

    public void Initialize(TransformCard transformCard) {
        this.transformCardController = transformCard;
    }
    protected override void PickUp() {
        // TODO: Might need to change for "Picking up the player"
        this.transformCardController.UnTransform();
    }
    
    protected override void Sabotage() {
        this.transformCardController.UnTransform();
    }
}
