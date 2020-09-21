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

    public void TryPickUp(PlayerController player) {
        if (_interactable) {
            PickUp(player);
            OnInteract();
        }
    }

    protected virtual void PickUp(PlayerController player) {

    }
    
    public void TryRepair() {
        if (_interactable) {
            Repair();
            OnInteract();
        }
    }
    
    protected virtual void Repair() {

    }

    public void TrySabotage() {
        if (_interactable) {
            Sabotage();
            OnInteract();
        }
    }

    protected virtual void Sabotage() {

    }

    public bool IsInteractable() {
        return _interactable;
    }
    
    protected void OnInteract() {
        UpdatePrompts();
    }
    
    // Tells nearby players to update their prompts UI
    protected void UpdatePrompts() {
        Collider[] nearbyPlayers = Physics.OverlapSphere(transform.position, _collider.radius, 1 << LayerMask.NameToLayer("Player"));
        foreach (Collider player in nearbyPlayers) {
            player.GetComponent<PlayerController>().UpdatePrompts();
        }
    }
    
    public void ShowPrompts(bool show) {
        if (show) {
            _interactionPrompts.SetActive(true);
        } else {
            _interactionPrompts.SetActive(false);
        }
    }
}
