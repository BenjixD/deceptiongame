using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalProp : InteractableObject {
    public Prop prop;
    private SpriteRenderer _spriteRenderer;
    [HideInInspector] public Sprite sprite;

    protected override void Start() {
        base.Start();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        sprite = _spriteRenderer.sprite;
        if (prop != null) {
            SetProp(prop);
        }
    }

    public void SetProp(Prop prop) {
        this.prop = prop;
        _spriteRenderer.sprite = prop.sprite;
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
    
    protected override void Sabotage(PlayerController player) {
        Debug.Log("Destroyed prop: " + gameObject.name);
        _spriteRenderer.sprite = prop.destroyedSprite;
        _interactable = false;
    }
}
