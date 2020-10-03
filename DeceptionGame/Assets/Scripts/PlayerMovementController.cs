using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class PlayerMovementController : Bolt.EntityBehaviour<INetPlayerState> {
    [Header("References")]
    public PlayerController playerController;
    [SerializeField] private Rigidbody _rb = null;

    [Space]

    [SerializeField] private float _moveSpeed = 0;
    [HideInInspector] public Vector3 moveDirection = Vector3.zero;

    public override void Attached()
    {
        // Update state position to be what was instantiated by Bolt
        state.SetTransforms(state.transform, transform);
    }

    public override void SimulateOwner() {
        if (playerController.IsControllable()) {
            // Movement
            moveDirection.x = Input.GetAxisRaw("Horizontal");
            moveDirection.z = Input.GetAxisRaw("Vertical");
            moveDirection.Normalize();
        }  else {
            moveDirection = Vector3.zero;
        }
        _rb.velocity = moveDirection * _moveSpeed;
    }

    void Update() { 
#if UNITY_EDITOR
        // For Running Locally without Client/Server Setup
        if(!BoltNetwork.IsRunning) {
            SimulateOwner();
        }
#endif
    }
}
