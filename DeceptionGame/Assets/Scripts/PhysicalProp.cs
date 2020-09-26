using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalProp : InteractableObject {
    private SpriteRenderer _spriteRenderer;
    [HideInInspector] public Sprite sprite;
    public Sprite destroyedSprite;

    protected override void Start() {
        base.Start();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        sprite = _spriteRenderer.sprite;
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
    }
    
    protected override void Sabotage() {
        Debug.Log("Destroyed prop: " + gameObject.name);
        _spriteRenderer.sprite = destroyedSprite;
        _interactable = false;
    }
}
