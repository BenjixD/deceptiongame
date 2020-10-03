using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerOnCollisionObject))]
public class StunObject : MonoBehaviour, ITriggerObject
{

    public float stunDuration = 1f;

    public void Initialize(float stunDuration) {
        this.stunDuration = stunDuration;
    }

    public void TriggerEntered(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        playerController.playerStats.AddTimedStatusAilment(StatusAilment.STATUS_AILMENT_ROOTED, stunDuration);
        playerController.GetComponent<Rigidbody>().velocity = Vector3.zero;

        Debug.Log("Stun to: " + other.gameObject.name + ": " + stunDuration);
    }
}
