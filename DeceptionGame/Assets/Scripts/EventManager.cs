using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : InteractableObject {
    [Header("References")]
    public EventUI eventUI;
    public List<Prop> temporaryPropLibrary;

    [Space]

    [SerializeField, Tooltip("The time (in seconds) between an event starting and the event failing.")]
    private float _eventTimer = 0;
    [SerializeField, Tooltip("The time (in seconds) between an event ending and the next event starting.")]
    private float _eventCooldown = 0;
    [SerializeField, Tooltip("The time (in seconds) it takes to complete an event after all participants have gathered.")]
    private float _eventCompletionTime = 0;
    [Tooltip("The maximum number of props/players required for an event. Temporary variable.")]
    public int tempMaxPropsRequired = 0;
    private bool _eventActive = false;
    private bool _eventProgressing = false;
    private Coroutine _currEventCountdown;
    private Objective _currentObjective;
    private int _objectivesComplete;
    private int _objectivesFailed;
    private int _totalObjectives = 5;

    protected override void Start() {
        base.Start();
        _objectivesComplete = 0;
        _objectivesFailed = 0;

        // TODO: move elsewhere
        _currEventCountdown = StartCoroutine(StartEvent());
    }

    private Objective GenerateObjective() {
        // TODO: make objective generation more sophisticated
        List<Prop> selected = new List<Prop>();
        int quantity = Random.Range(1, tempMaxPropsRequired + 1);
        for (int i = 0; i < quantity; i++) {
            Prop randomProp = temporaryPropLibrary[Random.Range(0, temporaryPropLibrary.Count)];
            selected.Add(randomProp);
        }
        int saboteursNeededToFail = 1;
        return new Objective(selected, _eventCompletionTime, saboteursNeededToFail);
    }

    private IEnumerator StartEvent() {
        _eventActive = true;
        _interactable = true;
        _currentObjective = GenerateObjective();
        eventUI.SetObjective(_currentObjective);

        // temporaryEventPopup.SetActive(true);
        UpdatePrompts();

        // Fail event after time runs out
        yield return new WaitForSeconds(_eventTimer);
        EndEvent();
    }
    
    protected override void Repair(PlayerController player) {
        JoinEvent(player, false);
    }

    protected override void Sabotage(PlayerController player) {
        Debug.Log("Faking event");
        // TODO: verify player is on red team first
        JoinEvent(player, true);
    }

    private void JoinEvent(PlayerController player, bool sabotage) {
        if (_eventActive) {
            Prop prop = player.GetProp();
            if (prop != null && _currentObjective.ValidParticipant(player)) {
                eventUI.CheckOffProp(player, sabotage);
                _currentObjective.ContributeProp(player, sabotage);
                CheckObjectiveProgress();
                UpdatePrompts();
            } else {
                Debug.Log("Player has improper prop or is already participating in the event");
            }
        }
    }

    public void TryLeaveEvent(PlayerController player) {
        if (_currentObjective.HasParticipant(player)) {
            _currentObjective.RemoveContribution(player);
            eventUI.UncheckProp(player);
            UpdatePrompts();
        }
    }

    private void CheckObjectiveProgress() {
        if (_currentObjective.RequirementsFulfilled()) {
            _eventProgressing = true;
            StartCoroutine(ProgressEvent());
        }
    }

    private IEnumerator ProgressEvent() {
        while (_eventProgressing && !_currentObjective.ObjectiveOver()) {
            _currentObjective.progress += Time.deltaTime;
            eventUI.SetProgress(_currentObjective.GetProgressPercent());
            yield return null;
        }

        // Finish event if it was completed, or reset progress if it was interrupted
        if (_currentObjective.ObjectiveOver()) {
            EndEvent();
        } else {
            _currentObjective.progress = 0;
        }
    }

    private void EndEvent() {
        _eventActive = false;
        _interactable = false;
        eventUI.ClearUI();
        UpdatePrompts();
        
        // End countdown of ended event
        StopCoroutine(_currEventCountdown);

        if (_currentObjective.ObjectiveOver()) {
            if (_currentObjective.ObjectiveComplete()) {
                Debug.Log("Event completed");
                _objectivesComplete++;
            } else {
                Debug.Log("Event failed (sabotaged)");
                _objectivesFailed++;
            }
        } else {
            Debug.Log("Event failed (timeout)");
            _objectivesFailed++;
        }
        
        if (!GameOver()) {
            StartCoroutine(PrepareNextEvent());
        }
    }

    private bool GameOver() {
        float majorityQuantity = _totalObjectives / 2f;
        if (_objectivesComplete > majorityQuantity) {
            Debug.Log("Game over: blue team wins " + _objectivesComplete + "-" + _objectivesFailed);
            return true;
        } else if (_objectivesFailed > majorityQuantity) {
            Debug.Log("Game over: red team wins " + _objectivesFailed + "-" + _objectivesComplete);
            return true;
        }
        return false;
    }

    private IEnumerator PrepareNextEvent() {
        yield return new WaitForSeconds(_eventCooldown);
        _currEventCountdown = StartCoroutine(StartEvent());
    }
}
