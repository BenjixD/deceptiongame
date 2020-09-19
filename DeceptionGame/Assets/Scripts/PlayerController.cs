using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Rigidbody _rb = null;

    [Space]

    [SerializeField] private float _moveSpeed = 0;
    private Vector3 _moveDirection = Vector3.zero;
    [SerializeField] private float _interactionRadius = 0;
    private Collider[] _nearbyInteractables = null;

    private void Update() {
        // Movement
        _moveDirection.x = Input.GetAxisRaw("Horizontal");
        _moveDirection.z = Input.GetAxisRaw("Vertical");
        _moveDirection.Normalize();

        if (Input.GetButtonDown("PickUp")) {
            TryPickUpProp();
        }
        
        if (Input.GetButtonDown("Repair")) {
            TryDoEvent();
        }
    }

    private void FixedUpdate() {
        _rb.velocity = _moveDirection * _moveSpeed;
    }

    // Tries to pick up nearest prop that's in range
    private void TryPickUpProp() {
        _nearbyInteractables = Physics.OverlapSphere(transform.position, _interactionRadius, 1 << LayerMask.NameToLayer("Prop"));
        TryInteract();
    }
    
    private void TryDoEvent() {
        _nearbyInteractables = Physics.OverlapSphere(transform.position, _interactionRadius, 1 << LayerMask.NameToLayer("Event"));
        TryInteract();
    }

    private void TryInteract() {
        Transform interactableTransform = GetClosestInteractable();
        if (interactableTransform != null) {
            IInteractable interactable = interactableTransform.GetComponent<IInteractable>();
            if (interactable == null) {
                Debug.LogWarning("Interactable object doesn't implement IInteractable");
                return;
            }
            interactable.Interact();
        }
    }

    private Transform GetClosestInteractable() {
        if (_nearbyInteractables.Length == 0) {
            return null;
        }
        if (_nearbyInteractables.Length == 1) {
            return _nearbyInteractables[0].transform;
        }
        Collider closestInteractable = _nearbyInteractables[0];
        float shortestDistance = Vector2.Distance(transform.position, closestInteractable.transform.position);
        float currDistance = 0;
        for (int i = 1; i < _nearbyInteractables.Length; i++) {
            currDistance = Vector2.Distance(transform.position, _nearbyInteractables[i].transform.position);
            if (currDistance < shortestDistance) {
                closestInteractable = _nearbyInteractables[i];
                shortestDistance = currDistance;
            }
        }
        return closestInteractable.transform;
    }
    
    private void OnDrawGizmosSelected() {
        // Draw interaction range in editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _interactionRadius);
    }
}
