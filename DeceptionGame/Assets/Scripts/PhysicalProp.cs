using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalProp : InteractableObject {
    public Sprite sprite;

    protected override void Start() {
        base.Start();
        sprite = GetComponent<SpriteRenderer>().sprite;
    }

    protected override void PickUp() {
        Debug.Log("Picked up prop: " + gameObject.name);
        _interactable = false;
        gameObject.SetActive(false);
    }

    public void Drop(Vector3 dropLocation) {
        transform.position = dropLocation;
        _interactable = true;
        gameObject.SetActive(true);
        UpdatePrompts();
    }
    
    protected override void Sabotage() {
        // TODO: leave behind debris
        Debug.Log("Destroyed prop: " + gameObject.name);
        _interactable = false;
    }
}
