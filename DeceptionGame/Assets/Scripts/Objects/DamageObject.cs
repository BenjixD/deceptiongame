using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerOnCollisionObject))]
public class DamageObject : MonoBehaviour, ITriggerObject
{
    public float damageAmount = 1;

    // Controller are necessary only if you want to remove from playList after 
    public void Initialize(float damageAmount) {
        this.damageAmount = damageAmount;
    }

    public void TriggerEntered(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        playerController.playerStats.DealDamage(this.damageAmount);
        Debug.Log("Deal damage to: " + other.gameObject.name + ": " + damageAmount); // TODO: Add player health
    }
}
