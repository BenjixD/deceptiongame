using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class InteractableObject : MonoBehaviour {
    [SerializeField] protected GameObject _interactionPrompts;
    protected bool _interactable;
    private SphereCollider _collider;     // Collider identifying interaction range

    protected virtual void Start() {
        ShowPrompts(false);
        _interactable = true;
        _collider = GetComponent<SphereCollider>();
    }

    // Tries to pick up this prop and returns true iff successful
    public bool TryPickUp() {
        if (_interactable) {
            PickUp();
            OnInteract();
            return true;
        }
        return false;
    }

    protected virtual void PickUp() {

    }
    
    public void TryRepair(PlayerController player) {
        if (_interactable) {
            Repair(player);
            OnInteract();
        }
    }
    
    protected virtual void Repair(PlayerController player) {

    }

    public virtual void TrySabotage(PlayerController player) {
        if (_interactable) {
            Sabotage(player);
            OnInteract();
        }
    }

    protected virtual void Sabotage(PlayerController player) {

    }

    public bool IsInteractable() {
        return _interactable;
    }
    
    protected void OnInteract() {
        UpdatePrompts();
    }
    
    // Tells nearby players to update their prompts UI
    protected void UpdatePrompts() {
        Collider[] nearbyPlayers = Physics.OverlapSphere(transform.position + _collider.center, _collider.radius, 1 << LayerMask.NameToLayer("Player"));
        foreach (Collider player in nearbyPlayers) {
            player.GetComponent<PlayerController>().UpdatePrompts();
        }
    }
    
    public void ShowPrompts(bool show) {
        if (show && _interactable) {
            _interactionPrompts.SetActive(true);
        } else {
            _interactionPrompts.SetActive(false);
        }
    }
}
