using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerObject {
    void TriggerEntered(Collider other);
    
}

// Class for organizing triggers
public class TriggerOnCollisionObject : MonoBehaviour
{

    public bool allowFriendlyFire = false;
    public bool hitPlayersOnly = true;

    public bool destroyIfOutOfBounds = true;

    public float timeBeforeCanTrigger = 0;

    public bool destroyOnTrigger = false;

    protected List<ITriggerObject> triggerObjects;
    protected PlayerController owner;

    [Tooltip("Useful for objects that don't trigger instantly (e.g. traps)")]
    private float timeBeforeCanTriggerCounter = 0;

    private Action onTriggerEnterCallback;

    void Start() {
        triggerObjects = new List<ITriggerObject>(GetComponents<ITriggerObject>());
    }

    void Update() {
        if (timeBeforeCanTriggerCounter < timeBeforeCanTrigger) {
            timeBeforeCanTriggerCounter += Time.deltaTime;
        }
    }

    public void Initialize(PlayerController owner, Action triggerCallback = null) {
        this.owner = owner;
        this.onTriggerEnterCallback = triggerCallback;
    }

    public void OnTriggerEnter(Collider other) {
        if (timeBeforeCanTriggerCounter < timeBeforeCanTrigger) {
            return;
        }

        if (!allowFriendlyFire && other.gameObject == owner.gameObject) {
            return;
        }

        if (destroyIfOutOfBounds && other.tag == "OutOfBounds") {
            Destroy(this.gameObject);
            return;
        } 

        if (this.hitPlayersOnly && other.tag != "Player") {
            return;
        }

        foreach (ITriggerObject triggerObject in this.triggerObjects) {
            triggerObject.TriggerEntered(other);
        }

        if (this.onTriggerEnterCallback != null) {
            this.onTriggerEnterCallback();
        }

        if (this.destroyOnTrigger) {
            Destroy(this.gameObject);
        }
    }

    public static void InitializeDamageStunVelocityHelper(TriggerOnCollisionObject obj, PlayerController owner, float damage, float stunDuration, Vector3 velocity, Action triggerCallback = null) {
        obj.Initialize(owner, triggerCallback);

        DamageObject damageObject = obj.GetComponent<DamageObject>();
        StunObject stunObject = obj.GetComponent<StunObject>();
        MovingObject moveObject = obj.GetComponent<MovingObject>();
        if (damageObject) {
            damageObject.Initialize(damage);
        }

        if (stunObject) {
            stunObject.Initialize(stunDuration);
        }

        if (moveObject) {
            moveObject.Initialize(velocity);
        }
    }

}
