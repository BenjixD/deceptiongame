using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRequirements {
    public int required;
    public int collected;

    public PropRequirements(int required, int collected) {
        this.required = required;
        this.collected = collected;
    }
}

public class ParticipationInfo {
    public Prop prop;
    public bool sabotage;

    public ParticipationInfo(Prop prop, bool sabotage) {
        this.prop = prop;
        this.sabotage = sabotage;
    }
}

public class Objective {
    // The props involved in this objective, the quantity required to complete it, and the quantity currently collected
    private Dictionary<Prop, PropRequirements> _propChecklist = new Dictionary<Prop, PropRequirements>();
    // Dictionary of participants in this event and whether they chose to fail it
    private Dictionary<PlayerController, ParticipationInfo> _contributions = new Dictionary<PlayerController, ParticipationInfo>();
    public float progress;
    private float _timeToComplete;
    // The number of players/props currently involved in sabotaging this objective
    private int _saboteurs;
    private int _saboteursNeededToFail;

    public Objective(List<Prop> props, float timeToComplete, int saboteursNeededToFail) {
        _timeToComplete = timeToComplete;
        _saboteursNeededToFail = saboteursNeededToFail;
        progress = 0;
        _saboteurs = 0;
        foreach (Prop prop in props) {
            AddRequirement(prop);
        }
    }

    public Dictionary<Prop, PropRequirements> GetChecklist() {
        return _propChecklist;
    }

    private void AddRequirement(Prop prop) {
        if (!_propChecklist.ContainsKey(prop)) {
            _propChecklist.Add(prop, new PropRequirements(1, 0));
        } else {
            _propChecklist[prop].required++;
        }
    }

    public bool ValidParticipant(PlayerController player) {
        return !HasParticipant(player) && RequiresProp(player.GetProp());
    }

    public bool HasParticipant(PlayerController player) {
        return _contributions.ContainsKey(player);
    }

    private bool RequiresProp(Prop prop) {
        return _propChecklist.ContainsKey(prop) && !PropRequirementFulfilled(prop);
    }

    public void ContributeProp(PlayerController player, bool sabotage) {
        Prop prop = player.GetProp();
        _contributions.Add(player, new ParticipationInfo(prop, sabotage));
        _propChecklist[prop].collected++;
        if (sabotage) {
            _saboteurs++;
        }
    }

    public void RemoveContribution(PlayerController player) {
        ParticipationInfo info = _contributions[player];
        _propChecklist[info.prop].collected--;
        if (info.sabotage) {
            _saboteurs--;
        }
        _contributions.Remove(player);
    }

    private bool PropRequirementFulfilled(Prop prop) {
        return _propChecklist[prop].collected >= _propChecklist[prop].required;
    }

    public bool RequirementsFulfilled() {
        foreach (KeyValuePair<Prop, PropRequirements> requirement in _propChecklist) {
            if (!PropRequirementFulfilled(requirement.Key)) {
                return false;
            }
        }
        return true;
    }

    public float GetProgressPercent() {
        return progress / _timeToComplete;
    }

    public bool ObjectiveOver() {
        return progress >= _timeToComplete;
    }

    public bool ObjectiveComplete() {
        return ObjectiveOver() && _saboteurs < _saboteursNeededToFail;
    }
}
