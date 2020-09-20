using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : InteractableObject {
    [SerializeField, Tooltip("The time (in seconds) between an event starting and the event failing.")]
    private float _eventTimer = 0;
    [SerializeField, Tooltip("The time (in seconds) between an event ending and the next event starting.")]
    private float _eventCooldown = 0;
    private bool _eventActive = false;
    private Coroutine currEventCountdown;

    public GameObject temporaryEventPopup;

    protected override void Start() {
        base.Start();
        temporaryEventPopup.SetActive(false);

        // TODO: move elsewhere
        currEventCountdown = StartCoroutine(StartEvent());
    }

    private IEnumerator StartEvent() {
        _eventActive = true;
        _interactable = true;
        temporaryEventPopup.SetActive(true);

        // Fail event after time runs out
        yield return new WaitForSeconds(_eventTimer);
        Debug.Log("Event failed");
        temporaryEventPopup.SetActive(false);

        // TODO: set game over

        StartCoroutine(EndEvent());
    }
    
    protected override void Repair() {
        DoEvent();
    }

    protected override void Sabotage() {
        Debug.Log("Faking event");
        DoEvent();
    }

    private void DoEvent() {
        if (_eventActive) {
            // TODO: only complete after holding repair button for certain period of time
            Debug.Log("Event complete");
            StartCoroutine(EndEvent());
        }
    }

    private IEnumerator EndEvent() {
        _eventActive = false;
        _interactable = false;
        temporaryEventPopup.SetActive(false);
        // End countdown of completed event
        StopCoroutine(currEventCountdown);
        
        // TODO: check game is not over before continuing game
        yield return new WaitForSeconds(_eventCooldown);
        currEventCountdown = StartCoroutine(StartEvent());
    }
}
