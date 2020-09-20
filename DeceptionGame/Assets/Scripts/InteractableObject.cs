using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
    [SerializeField] protected GameObject _interactionPrompts;
    protected bool _interactable;

    protected virtual void Start() {
        ShowPrompts(false);
        _interactable = true;
    }

    public void TryPickUp() {
        if (_interactable) {
            PickUp();
            OnInteract();
        }
    }

    protected virtual void PickUp() {

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
        // Tell nearby players to update their prompts UI
        Collider[] nearbyPlayers = Physics.OverlapSphere(transform.position, 10f, 1 << LayerMask.NameToLayer("Player"));
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
