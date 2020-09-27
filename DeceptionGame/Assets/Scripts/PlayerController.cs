﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerHorizontalDirection {
    DEFAULT,
    LEFT,
    RIGHT
};

public enum PlayerVerticalDirection {
    DEFAULT,
    UP,
    DOWN
}

public class PlayerController : MonoBehaviour {
    [Header("References")]
    public ShowOnlyIfInRange showIfInRangeScript;
    public PlayerCardController cardController;

    public Transform throwLocation; // Might want to be its own class later on like in Mooks if we have too many...

    [SerializeField] private Rigidbody _rb = null;

    [Space]

    [SerializeField] private float _moveSpeed = 0;
    private Vector3 _moveDirection = Vector3.zero;
    // [SerializeField] private float _interactionRadius = 0;
    private List<InteractableObject> _nearbyInteractables = new List<InteractableObject>();
    private InteractableObject _closestInteractable;

    
    // TODO: Move this in PlayerInputController

    protected PlayerHorizontalDirection playerHorizontalDirection = PlayerHorizontalDirection.DEFAULT;
    protected PlayerVerticalDirection playerVerticalDirection = PlayerVerticalDirection.DEFAULT;


    public void Initialize() {
        if (GameManager.Instance.controller.mainPlayer != null && this.transform != GameManager.Instance.controller.mainPlayer) {
            this.enabled = false; // TODO: Change logic to be more sophisticated
        }

        this.cardController.Initialize(this);
    }

    private void Update() {
        // Movement
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        _moveDirection.x = inputX;
        _moveDirection.z = inputY;
        _moveDirection.Normalize();

        if (inputX == 0) {
            this.playerHorizontalDirection = PlayerHorizontalDirection.DEFAULT;
        } else {
            this.playerHorizontalDirection = inputX >= 0 ? PlayerHorizontalDirection.RIGHT : PlayerHorizontalDirection.LEFT;
        }

        if (inputY == 0) {
            this.playerVerticalDirection = PlayerVerticalDirection.DEFAULT;
        } else {
            this.playerVerticalDirection = inputY >= 0 ? PlayerVerticalDirection.UP : PlayerVerticalDirection.DOWN;
        }

        if (Input.GetButtonDown("PickUp")) {
            TryPickUpProp();
        } else if (Input.GetButtonDown("Repair")) {
            TryRepair();
        } else if (Input.GetButtonDown("Sabotage")) {
            TrySabotage();
        }

        this.cardController.UpdateActiveCards();

        if (Input.GetKeyDown(KeyCode.Space)) {
            this.cardController.PlayCardInHand(0);
        }
    }

    private void FixedUpdate() {
        _rb.velocity = _moveDirection * _moveSpeed;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Interactable")) {
            _nearbyInteractables.Add(other.GetComponent<InteractableObject>());
            UpdatePrompts();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Interactable")) {
            InteractableObject interactable = other.GetComponent<InteractableObject>();
            if (_nearbyInteractables.Contains(interactable)) {
                HidePrompts();
                _nearbyInteractables.Remove(interactable);
                UpdatePrompts();
            }
        }
    }

    public void UpdatePrompts() {
        HidePrompts();
        CleanNearbyInteractables();
        UpdateClosestInteractable();
        if (_closestInteractable != null) {
            InteractableObject interactable = _closestInteractable.GetComponent<InteractableObject>();
            if (interactable == null) {
                Debug.LogWarning("Interactable object doesn't have InteractableObject component");
                return;
            }
            interactable.ShowPrompts(true);
        }
    }

    public PlayerHorizontalDirection GetHorizontalDirection() {
        return this.playerHorizontalDirection;
    }

    public PlayerVerticalDirection GetVerticalDirection() {
        return this.playerVerticalDirection;
    }


    // Hide the prompts of all nearby objects
    private void HidePrompts() {
        foreach (InteractableObject interactable in _nearbyInteractables) {
            if (interactable != null) {
                interactable.ShowPrompts(false);
            }
        }
    }

    private void CleanNearbyInteractables() {
        for (int i = _nearbyInteractables.Count - 1; i >= 0; i--) {
            if (_nearbyInteractables[i] == null) {
                _nearbyInteractables.Remove(_nearbyInteractables[i]);
            }
        }
    }

    // Tries to pick up closest interactable
    private void TryPickUpProp() {
        if (_closestInteractable != null) {
            _closestInteractable.GetComponent<InteractableObject>().TryPickUp();
        }
    }
    
    private void TryRepair() {
        if (_closestInteractable != null) {
            _closestInteractable.GetComponent<InteractableObject>().TryRepair();
        }
    }

    private void TrySabotage() {
        if (_closestInteractable != null) {
            _closestInteractable.GetComponent<InteractableObject>().TrySabotage();
        }
    }

    private void UpdateClosestInteractable() {
        if (_nearbyInteractables.Count == 0) {
            _closestInteractable = null;
            return;
        }
        if (_nearbyInteractables.Count == 1 && _nearbyInteractables[0].IsInteractable()) {
            _closestInteractable = _nearbyInteractables[0];
            return;
        }
        _closestInteractable = null;
        float shortestDistance = 0;
        float currDistance = 0;
        for (int i = 0; i < _nearbyInteractables.Count; i++) {
            if (_nearbyInteractables[i].IsInteractable()) {
                currDistance = Vector2.Distance(transform.position, _nearbyInteractables[i].transform.position);
                if (currDistance < shortestDistance || _closestInteractable == null) {
                    _closestInteractable = _nearbyInteractables[i];
                    shortestDistance = currDistance;
                }
            }
        }
    }
}
