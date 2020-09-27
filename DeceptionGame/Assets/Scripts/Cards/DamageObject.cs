using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public int damageAmount = 1;
    private PlayerController owner;
    private Vector3 velocity;

    private Card controller;

    private bool removeFromPlayList;

    // Controller are necessary only if you want to remove from playList after 
    public void Initialize(Vector3 velocity, PlayerController owner, Card controller = null) {
        this.velocity = velocity;
        this.owner = owner;
        this.controller = controller;

        if (controller != null) {
            removeFromPlayList = true;
        }

    }

    void Update() {
        this.transform.position += velocity * Time.deltaTime;
    }

    public void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && other.gameObject != owner.gameObject) {
            Debug.Log("DEAL INSANE DAMAGE TO: " + other.gameObject.name + " " + damageAmount); // TODO: Add player health
        } else if (other.tag == "OutOfBounds") {
            if (removeFromPlayList) {
                owner.cardController.RemoveCardFromPlayList(controller);
            }
            Destroy(this.gameObject);
        }
    }
}
