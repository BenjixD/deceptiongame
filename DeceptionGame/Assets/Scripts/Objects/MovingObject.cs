using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerOnCollisionObject))]
public class MovingObject : MonoBehaviour, ITriggerObject
{
    private Vector3 velocity;

    public void Initialize(Vector3 velocity) {
        this.velocity = velocity;
    }

    public void TriggerEntered(Collider other)
    {
        // Do nothing
    }

    void Update() {
        this.transform.position += velocity * Time.deltaTime;
    }
}
