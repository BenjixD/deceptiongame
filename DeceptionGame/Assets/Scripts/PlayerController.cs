using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerController : MonoBehaviour {
    [Header("References")]
    public PlayerCardController cardController;

    public Transform throwLocation; // Might want to be its own class later on like in Mooks if we have too many...

    public PlayerStats playerStats;

    [SerializeField] private Rigidbody _rb = null;
    public PlayerHUD playerHUD;

    [Header("Controllers")]
    public PlayerMovementController mvController;
    public AnimationController animController;

    public bool debug_SpawnOnStart = false;

    private List<InteractableObject> _nearbyInteractables = new List<InteractableObject>();
    private InteractableObject _closestInteractable;
    private PhysicalProp _heldProp = null;
    private bool _animLocked;


    private void Start() {
        if (debug_SpawnOnStart) {
            Spawn();
        }
    }


    public void Initialize() {

        if (GameManager.Instance.controller.mainPlayer != null && this.transform != GameManager.Instance.controller.mainPlayer) {
            // Disable all Controllers
            mvController.enabled = false;
            this.enabled = false; // TODO: Change logic to be more sophisticated
        }

        this.playerStats.Initialize();

        this.cardController.Initialize(this);
        this.animController.Initialize();
        this.Spawn();
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

            if (Input.GetKeyDown(KeyCode.Space)) {
                this.cardController.PlayCardInHand(0);
            }


            // TODO: Find a better place to do this
            // Drop card
            if (Input.GetKeyDown(KeyCode.G) && EventSystem.current.IsPointerOverGameObject()) {
                Debug.Log("Drop card");
                this.TryDropCard();
            }
        }
        this.playerStats.CheckAilments();
        UpdateAnims();

    }

    private void TryDropCard() {

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        RaycastResult hitResult = raycastResults.Find(raycast => raycast.gameObject.tag == "UICardImage");

        if (hitResult.gameObject != null) {
            Debug.Log("Hit");
            UICardImage uiCardImage = hitResult.gameObject.GetComponent<UICardImage>();
            Card theCardReference = uiCardImage.card;
            this.cardController.RemoveCardFromHand(theCardReference);
            CardProp cardProp = Instantiate<CardProp>(GameManager.Instance.models.cardPropPrefab);
            cardProp.Initialize(theCardReference);
            cardProp.transform.position = this.transform.position;
            
        } else {
            Debug.Log("Missed");
        }
    }

    public bool IsControllable() {
        return !_animLocked && this.playerStats.CanPlayerMove();
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
	            InteractableObject interactable = other.GetComponent<InteractableObject>();
	            if (_nearbyInteractables.Contains(interactable)) {
	                HidePrompts();
	                _nearbyInteractables.Remove(interactable);
	                UpdatePrompts();
	            }
	
	            // Withdraw from events upon moving out of range
	            EventManager eventManager = other.GetComponent<EventManager>();
	            if (eventManager != null) {
	                TryLeaveEvent(eventManager);
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
                if (prop.carryPropOnPickup) {
                    AcquireProp(prop);
                }
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
        if (_heldProp == null) {
            return null;
        }
        return _heldProp.prop;
    }

    public void AcquireProp(PhysicalProp prop) {
        if (_heldProp != null) {
            LoseProp();
        }
        _heldProp = prop;
        playerHUD.PickUpProp(prop);
        animController.SetAnimation(1, "layered holding idle", true);
    }

    public void TryDropProp() {
        if (_heldProp != null && IsControllable()) {
            int track = animController.TakeFreeTrack();
            if (track != -1) {
                animController.AddToTrack(track, "drop", false, 0);
                animController.EndTrackAnims(track);
            }
            StartCoroutine(SetAnimationLock(animController.GetAnimationDuration("drop")));
            LoseProp();
        }
    }

    // Sets current prop down at player's location
    private void LoseProp() {
        // Drop prop
        _heldProp.Drop(transform.position);
        _nearbyInteractables.Remove(_heldProp.GetComponent<InteractableObject>());
        _heldProp = null;

        // If participating in an event, Withdraw from it
        EventManager eventManager = GetNearbyEvent();
        if (eventManager != null) {
            TryLeaveEvent(eventManager);
        }

        playerHUD.EmptyProp();
        animController.SetAnimation(1, "layered arm idle", true);
    }

    // Tries to return the first nearby event, or null if there isn't one
    private EventManager GetNearbyEvent() {
        EventManager eventManager = null;
        foreach (InteractableObject interactable in _nearbyInteractables) {
            eventManager = interactable.GetComponent<EventManager>();
            if (eventManager != null) {
                return eventManager;
            }
        }
        return null;
    }

    // Turn on animation lock for specified duration
    private IEnumerator SetAnimationLock(float duration) {
        _animLocked = true;
        yield return new WaitForSeconds(duration);
        _animLocked = false;
    }

    // Handles spawning and its animation
    private void Spawn() {
        float spawnAnimDuration = animController.GetAnimationDuration("spawn");
        StartCoroutine(SetAnimationLock(spawnAnimDuration));

        // Play the animation
        int track = animController.TakeFreeTrack();
        if (track != -1) {
            animController.AddToTrack(track, "spawn", false, 0);
            animController.EndTrackAnims(track);
        }
    }
}
