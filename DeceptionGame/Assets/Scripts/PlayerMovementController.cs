using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class PlayerMovementController : Bolt.EntityBehaviour<INetPlayerState> {
    [Header("References")]
    [SerializeField] private Rigidbody _rb = null;

    [Space]

    [SerializeField] private float _moveSpeed = 0;
    private Vector3 _moveDirection = Vector3.zero;

    public override void Attached()
    {
        // Update state position to be what was instantiated by Bolt
        state.SetTransforms(state.transform, transform);
    }

    public override void SimulateOwner() {
        // Movement
        _moveDirection.x = Input.GetAxisRaw("Horizontal");
        _moveDirection.z = Input.GetAxisRaw("Vertical");
        _moveDirection.Normalize();

        _rb.velocity = _moveDirection * _moveSpeed;
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
