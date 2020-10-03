using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventUI : MonoBehaviour {
    [Header("References")]
    public GameObject progressBar;
    public Image progressBarContent;
    public TextMeshProUGUI progressBarHelpText;
    public Transform propList;
    public PropListItem propListItemPrefab;
    
    private Dictionary<Prop, List<PropListItem>> _listItems = new Dictionary<Prop, List<PropListItem>>();
    // Dictionary of the list item each participant is tied to
    private Dictionary<PlayerController, PropListItem> _claimedItems = new Dictionary<PlayerController, PropListItem>();
    private int _participantsNeeded;

    private void Start() {
        progressBar.SetActive(false);
    }

    public void SetObjective(Objective objective) {
        ClearUI();
        _participantsNeeded = 0;
        propList.gameObject.SetActive(true);
        progressBar.SetActive(true);
        Dictionary<Prop, PropRequirements> propChecklist = objective.GetChecklist();
        foreach (KeyValuePair<Prop, PropRequirements> requirement in propChecklist) {
            _listItems.Add(requirement.Key, new List<PropListItem>());
            for (int i = 0; i < requirement.Value.required; i++) {
                PropListItem listItem = Instantiate(propListItemPrefab, propList);
                listItem.SetImage(requirement.Key.sprite);
                _listItems[requirement.Key].Add(listItem);
                _participantsNeeded++;
            }
        }
        SetProgress(0);
        UpdateProgressBarHelpText();
    }

    private void UpdateProgressBarHelpText() {
        if (_participantsNeeded > 0) {
            progressBarHelpText.text = _participantsNeeded + " more needed to start event";
        } else {
            progressBarHelpText.text = "";
        }
    }

    public void CheckOffProp(PlayerController player, bool sabotage) {
        List<PropListItem> itemList = _listItems[player.GetProp()];
        for (int i = 0; i < itemList.Count; i++) {
            if (!itemList[i].fulfilled) {
                itemList[i].Check(sabotage);
                _claimedItems.Add(player, itemList[i]);
                break;
            }
        }
        _participantsNeeded--;
        UpdateProgressBarHelpText();
    }

    public void UncheckProp(PlayerController player) {
        PropListItem item = _claimedItems[player];
        item.Uncheck();
        _claimedItems.Remove(player);
        _participantsNeeded++;
        UpdateProgressBarHelpText();
    }

    public void SetProgress(float value) {
        progressBarContent.fillAmount = value;
    }

    public void ClearUI() {
        propList.gameObject.SetActive(false);
        progressBar.SetActive(false);

        // Destroy required items UI
        foreach (KeyValuePair<Prop, List<PropListItem>> listItemsPairs in _listItems) {
            for (int i = listItemsPairs.Value.Count - 1; i >= 0; i--) {
                Destroy(listItemsPairs.Value[i].gameObject);
            }
        }
        _listItems.Clear();
        
        _claimedItems.Clear();
    }
}
