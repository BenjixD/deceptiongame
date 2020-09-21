using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalProp : InteractableObject {
    protected override void PickUp() {
        // TODO: process pickup (give to player who picked it up, etc.)
        Debug.Log("Picked up prop: " + gameObject.name);
        _interactable = false;
        Destroy(gameObject);
    }
    
    protected override void Sabotage() {
        // TODO: leave behind debris
        Debug.Log("Destroyed prop: " + gameObject.name);
        _interactable = false;
    }
}
