using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public int damageAmount = 1;
    private PlayerController owner;
    private Vector3 velocity;

    private SpawnDamageObjectCard controller;

    public void Initialize(PlayerController owner, SpawnDamageObjectCard controller, Vector3 velocity) {
        this.owner = owner;
        this.controller = controller;
        this.velocity = velocity;
    }

    void Update() {

        this.transform.position += velocity * Time.deltaTime;
    }

    public void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && other.gameObject != owner.gameObject) {
            Debug.Log("DEAL INSANE DAMAGE TO: " + other.gameObject.name + " " + damageAmount); // TODO: Add player health
        } else if (other.tag == "OutOfBounds") {
            owner.cardController.RemoveCardFromPlayList(controller);
            Destroy(this.gameObject);
        }
    }
}
