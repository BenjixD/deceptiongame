using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : InteractableObject {
    [Header("References")]
    public EventUI eventUI;
    public EventHistoryUI eventHistoryUI;
    public List<Prop> temporaryPropLibrary;

    [Space]

    [SerializeField, Tooltip("The time (in seconds) between an event starting and the event failing.")]
    private float _eventTimer = 0;
    [SerializeField, Tooltip("The time (in seconds) between an event ending and the next event starting.")]
    private float _eventCooldown = 0;
    [SerializeField, Tooltip("The time (in seconds) it takes to complete an event after all participants have gathered.")]
    private float _eventCompletionTime = 0;
    [Tooltip("The maximum number of players in this game. Temporary variable for testing.")]
    public int tempNumPlayers = 1;
    private bool _eventProgressing = false;
    private Coroutine _currEventCountdown;
    private Objective _currentObjective;
    private int _objectivesComplete;
    private int _objectivesFailed;
    private int _totalObjectives = 5;

    // Dictionary mapping ObjectiveParams (participants required and saboteurs needed for failure for each event) to total number of players.
    private Dictionary<int, ObjectiveParams[]> _objectiveParams =
        new Dictionary<int, ObjectiveParams[]>() {
            {1, new ObjectiveParams[] { new ObjectiveParams(1, 1), new ObjectiveParams(1, 1), new ObjectiveParams(1, 1), new ObjectiveParams(1, 1), new ObjectiveParams(1, 1) }}, // 1 player for testing purposes
            {4, new ObjectiveParams[] { new ObjectiveParams(2, 1), new ObjectiveParams(3, 1), new ObjectiveParams(2, 1), new ObjectiveParams(3, 1), new ObjectiveParams(3, 1) }}
        };

    protected override void Start() {
        base.Start();
        _objectivesComplete = 0;
        _objectivesFailed = 0;
        eventHistoryUI.InitializeBars(_objectiveParams[tempNumPlayers]);

        // TODO: move elsewhere
        _currEventCountdown = StartCoroutine(StartEvent());
    }


    private ObjectiveParams GetObjectiveParams(int totalPlayers) {
        if (!_objectiveParams.ContainsKey(totalPlayers)) {
            Debug.LogWarning("Events not configured for game with " + totalPlayers + " players");
            return null;
        }
        int eventNumber = _objectivesComplete + _objectivesFailed;
        if (eventNumber >= _objectiveParams[totalPlayers].Length) {
            Debug.LogWarning("No params found for round " + (eventNumber + 1) + " for game with " + totalPlayers + " players");
            return null;
        }
        return _objectiveParams[totalPlayers][eventNumber];
    }

    private Objective GenerateObjective() {
        // TODO: get number of players properly
        ObjectiveParams objectiveParams = GetObjectiveParams(tempNumPlayers);
        List<Prop> selected = new List<Prop>();

        // Randomize what props are needed
        int quantity = objectiveParams.participants;
        for (int i = 0; i < quantity; i++) {
            Prop randomProp = temporaryPropLibrary[Random.Range(0, temporaryPropLibrary.Count)];
            selected.Add(randomProp);
        }
        return new Objective(objectiveParams, selected, _eventCompletionTime);
    }

    private IEnumerator StartEvent() {
        _interactable = true;
        _currentObjective = GenerateObjective();
        eventUI.SetObjective(_currentObjective);
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

    public bool EventActive() {
        return _currentObjective != null;
    }

    private void JoinEvent(PlayerController player, bool sabotage) {
        if (EventActive()) {
            Prop prop = player.GetProp();
            if (prop != null && _currentObjective.ValidParticipant(player)) {
                eventUI.CheckOffProp(player, sabotage);
                _currentObjective.ContributeProp(player, sabotage);
                CheckObjectiveProgress();
                UpdatePrompts();
            } else {
                Debug.Log("Cannot repair: player has improper prop or is already participating in the event");
            }
        }
    }

    public void TryLeaveEvent(PlayerController player) {
        if (EventActive() &&_currentObjective.HasParticipant(player)) {
            eventUI.UncheckProp(player);
            _currentObjective.RemoveContribution(player);
            _currentObjective.progress = 0;
            _eventProgressing = false;
            UpdateProgress();
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
            UpdateProgress();
            yield return null;
        }

        if (EventActive()) {
            // Finish event if it was completed, or reset progress if it was interrupted
            if ( _currentObjective.ObjectiveOver()) {
                EndEvent();
            } else {
                _currentObjective.progress = 0;
            }
        }
    }

    private void UpdateProgress() {
        eventUI.SetProgress(_currentObjective.GetProgressPercent());
    }

    private void EndEvent() {
        if (!EventActive()) {
            return;
        }
        _eventProgressing = false;
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

        // Update event history
        int barIndex = _objectivesComplete + _objectivesFailed - 1;
        eventHistoryUI.SetBar(barIndex, _currentObjective);
        _currentObjective = null;

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
