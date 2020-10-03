using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventHistoryBar : MonoBehaviour {
    [Header("References")]
    
    [SerializeField] private Image _image = null;
    [SerializeField] private TextMeshProUGUI paramInfo = null;

    [Space]

    // Temporary colours for placeholder UI
    [SerializeField] private Color _tempBlue = Color.white;
    [SerializeField] private Color _tempRed = Color.white;

    public void Initialize(ObjectiveParams objectiveParams) {
        paramInfo.text = objectiveParams.participants + " participants   " + objectiveParams.saboteursNeededToFail + " needed to fail";
    }

    public void SetBar(Objective objective) {
        if (objective.ObjectiveComplete()) {
            _image.color = _tempBlue;
        } else {
            _image.color = _tempRed;
        }
        paramInfo.color = Color.white;
        // TODO: show other information, like the participants of the event
    }
}
