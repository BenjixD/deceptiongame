using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventHistoryUI : MonoBehaviour {
    [SerializeField] private EventHistoryBar[] _eventBars = null;

    public void InitializeBars(ObjectiveParams[] objectiveParams) {
        for (int i = 0; i < objectiveParams.Length; i++) {
            _eventBars[i].Initialize(objectiveParams[i]);
        }
    }

    public void SetBar(int barIndex, Objective objective) {
        if (barIndex >= _eventBars.Length) {
            Debug.LogWarning("Tried to set event history bar that does not exist");
            return;
        }
        _eventBars[barIndex].SetBar(objective);
    }
}
