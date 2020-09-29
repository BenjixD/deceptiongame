using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventHistoryUI : MonoBehaviour {
    [SerializeField] private Image[] _eventBars = null;
    
    // Temporary colours for placeholder UI
    [SerializeField] private Color _tempBlue;
    [SerializeField] private Color _tempRed;

    public void SetBar(int barIndex, Objective objective) {
        if (barIndex >= _eventBars.Length) {
            Debug.LogWarning("Tried to set event history bar that does not exist");
            return;
        }
        if (objective.ObjectiveComplete()) {
            _eventBars[barIndex].color = _tempBlue;
        } else {
            _eventBars[barIndex].color = _tempRed;
        }
        // TODO: show other information, like the participants of the event
    }
}
