using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("References")]
    public ShowOnlyIfInRange showIfInRangeScript;

    [Space]

    [Header("Controllers")]
    public PlayerMovementController _mvController;

    [Space]

    [SerializeField] private float _moveSpeed = 0;
    private Vector3 _moveDirection = Vector3.zero;
    // [SerializeField] private float _interactionRadius = 0;
    private List<InteractableObject> _nearbyInteractables = new List<InteractableObject>();
    private InteractableObject _closestInteractable;

    public void Initialize(Transform mainPlayerTransform) {
        if (mainPlayerTransform != null) {
            // This is NOT the main player
            showIfInRangeScript.Initialize(mainPlayerTransform);
            // Disable all Controllers
            _mvController.enabled = false;
            this.enabled = false; // TODO: Change logic to be more sophisticated
        } else {
            
        }
    }

    private void GetControllerComponents() {
        // Get all Controller Components
        _mvController = GetComponent<PlayerMovementController>();
    }

    private void Update() {
        if (Input.GetButtonDown("PickUp")) {
            TryPickUpProp();
        } else if (Input.GetButtonDown("Repair")) {
            TryRepair();
        } else if (Input.GetButtonDown("Sabotage")) {
            TrySabotage();
        }
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
