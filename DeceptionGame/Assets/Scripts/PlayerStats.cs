using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Note: Status ailments don't stack. Use cards for that
public class StatusAilment {
    public string name;
    public bool timedStatusAilment;
    public float endTime;

    public StatusAilment(string name, bool timedStatusAilment = false, float endTime = 0) {
        this.name = name;
        this.timedStatusAilment = timedStatusAilment;
        this.endTime = endTime;
    }

    public static string STATUS_AILMENT_STUNNED = "Stunned";
    public static string STATUS_AILMENT_ROOTED = "Rooted";
    public static string STATUS_AILMENT_POISON = "Poisoned";
};

public class PlayerStats : MonoBehaviour
{

    public float health = 0;
    public  float maxHealth = 100;
    private Dictionary<string, StatusAilment> statusAilments = new Dictionary<string, StatusAilment>();

    public void Initialize() {
        this.health = maxHealth;
    }

    public void CheckAilments() {
        List<string> keysToRemove = new List<string>();
        foreach (KeyValuePair<string, StatusAilment> ailmentPair in this.statusAilments) {
            if (Time.time >= ailmentPair.Value.endTime) {
                keysToRemove.Add(ailmentPair.Key);
            }
        }

        foreach (string key in keysToRemove) {
            this.statusAilments.Remove(key);
        }
    }


    public bool HasStatusAilment(string name) {
        return this.statusAilments.ContainsKey(name);
    }

    public void AddStatusAilment(StatusAilment ailment) {
        this.statusAilments[ailment.name] = ailment;
    }

    public void AddTimedStatusAilment(string ailmentName, float timeInSeconds) {
        float endTime = Time.time + timeInSeconds;
        if (this.statusAilments.ContainsKey(ailmentName)) {
            this.statusAilments[ailmentName].endTime = Mathf.Max(this.statusAilments[ailmentName].endTime, endTime);
        } else {
            StatusAilment newAilment = new StatusAilment(ailmentName, true, endTime);
        }
    }

    public void AddPermanentStatusAilment(string ailmentName) {
        StatusAilment newAilment = new StatusAilment(ailmentName);
        this.statusAilments[ailmentName] = newAilment;
    }


    public bool CanPlayerMove() {
        return !this.HasStatusAilment(StatusAilment.STATUS_AILMENT_STUNNED) && !this.HasStatusAilment(StatusAilment.STATUS_AILMENT_ROOTED);
    }
}
