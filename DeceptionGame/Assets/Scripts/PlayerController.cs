using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("References")]
    public ShowOnlyIfInRange showIfInRangeScript;
    public PlayerHUD playerHUD;

    [Space]

    [Header("Controllers")]
    public PlayerMovementController mvController;
    public AnimationController animController;

    [Space]

    [SerializeField] private float _moveSpeed = 0;
    private Vector3 _moveDirection = Vector3.zero;
    // [SerializeField] private float _interactionRadius = 0;
    private List<InteractableObject> _nearbyInteractables = new List<InteractableObject>();
    private InteractableObject _closestInteractable;
    private PhysicalProp _heldProp = null;
    private bool _animLocked;

    public void Initialize(Transform mainPlayerTransform) {
        if (mainPlayerTransform != null) {
            // This is NOT the main player
            showIfInRangeScript.Initialize(mainPlayerTransform);
            // Disable all Controllers
            mvController.enabled = false;
            this.enabled = false; // TODO: Change logic to be more sophisticated
        } else {
            
        }
    }

    private void GetControllerComponents() {
        // Get all Controller Components
        mvController = GetComponent<PlayerMovementController>();
    }

    private void Update() {
        if (IsControllable()) {
            if (Input.GetButtonDown("PickUp")) {
                TryPickUpProp();
            } else if (Input.GetButtonDown("Repair")) {
                TryRepair();
            } else if (Input.GetButtonDown("Sabotage")) {
                TrySabotage();
            }
        }
        UpdateAnims();
    }

    public void UpdateAnims() {
        if (mvController.moveDirection == Vector3.zero) {
            // Idle anims
            if (animController.TrackIsPlayingAnim(0, "walking")) {
                animController.SetAnimation(0, "idle", true);
                if (_heldProp == null) {
                    animController.SetAnimation(1, "layered arm idle", true);
                } else {
                    animController.SetAnimation(1, "layered holding idle", true);
                }
            }
        } else {
            // Walking anims
            if (animController.TrackIsPlayingAnim(0, "idle")) {
                animController.SetAnimation(0, "walking", true);
                if (_heldProp == null) {
                    animController.SetAnimation(1, "layered arm walking", true);
                } else {
                    animController.SetAnimation(1, "layered holding walking", true);
                }
            }
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
            Debug.Log("OnTriggerExit: " + other.gameObject.name);
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
            PhysicalProp prop = _closestInteractable.GetComponent<PhysicalProp>();
            if (prop != null && prop.TryPickUp()) {
                int track = animController.TakeFreeTrack();
                if (track != -1) {
                    animController.AddToTrack(track, "pick up", false, 0);
                    animController.EndTrackAnims(track);
                }
                StartCoroutine(SetAnimationLock(animController.GetAnimationDuration("pick up")));
                AcquireProp(prop);
            }
        }
    }
    
    private void TryRepair() {
        if (_closestInteractable != null) {
            _closestInteractable.GetComponent<InteractableObject>().TryRepair(this);
        }
    }

    private void TrySabotage() {
        if (_closestInteractable != null) {
            _closestInteractable.GetComponent<InteractableObject>().TrySabotage(this);
        }
    }

    private void TryLeaveEvent(EventManager eventManager) {
        eventManager.TryLeaveEvent(this);
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

    public Prop GetProp() {
        return _heldProp.prop;
    }

    public void AcquireProp(PhysicalProp prop) {
        if (_heldProp != null) {
            DropProp();
        }
        _heldProp = prop;
        playerHUD.SetPropImage(prop.sprite);
    }

    // Drops current prop at player's location
    private void DropProp() {
        _heldProp.Drop(transform.position);
        _nearbyInteractables.Remove(_heldProp.GetComponent<InteractableObject>());
        _heldProp = null;
    }

    // Turn on animation lock for specified duration
    private IEnumerator SetAnimationLock(float duration) {
        _animLocked = true;
        yield return new WaitForSeconds(duration);
        _animLocked = false;
    }
}
