using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalProp : MonoBehaviour, IInteractable {
    public void Interact() {
        OnPickUp();
    }

    private void OnPickUp() {
        // TODO: process pickup (give to player who picked it up, etc.)
        Debug.Log("Picked up prop: " + gameObject.name);
        Destroy(gameObject);
    }
}
