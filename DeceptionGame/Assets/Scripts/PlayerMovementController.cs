using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public enum PlayerHorizontalDirection {
    DEFAULT,
    LEFT,
    RIGHT
};

public enum PlayerVerticalDirection {
    DEFAULT,
    UP,
    DOWN
}


public class PlayerMovementController : Bolt.EntityBehaviour<INetPlayerState> {
    [Header("References")]
    public PlayerController playerController;
    [SerializeField] private Rigidbody _rb = null;

    [Space]

    [SerializeField] private float _moveSpeed = 0;
    [HideInInspector] public Vector3 moveDirection = Vector3.zero;


    protected PlayerHorizontalDirection playerHorizontalDirection = PlayerHorizontalDirection.DEFAULT;
    protected PlayerVerticalDirection playerVerticalDirection = PlayerVerticalDirection.DEFAULT;




    public PlayerHorizontalDirection GetHorizontalDirection() {
        return this.playerHorizontalDirection;
    }

    public PlayerVerticalDirection GetVerticalDirection() {
        return this.playerVerticalDirection;
    }


    public override void Attached()
    {
        // Update state position to be what was instantiated by Bolt
        state.SetTransforms(state.transform, transform);
    }

    public override void SimulateOwner() {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        if (playerController.IsControllable()) {
            // Movement
            moveDirection.x = inputX;
            moveDirection.z = inputY;
            moveDirection.Normalize();
        }  else {
            moveDirection = Vector3.zero;
        }
        _rb.velocity = moveDirection * _moveSpeed;

        if (inputX == 0) {
            this.playerHorizontalDirection = PlayerHorizontalDirection.DEFAULT;
        } else {
            this.playerHorizontalDirection = inputX >= 0 ? PlayerHorizontalDirection.RIGHT : PlayerHorizontalDirection.LEFT;
        }

        if (inputY == 0) {
            this.playerVerticalDirection = PlayerVerticalDirection.DEFAULT;
        } else {
            this.playerVerticalDirection = inputY >= 0 ? PlayerVerticalDirection.UP : PlayerVerticalDirection.DOWN;
        }

        if (moveDirection.magnitude != 0) {
            Messenger.Broadcast(Messages.OnMoveMainPlayer);
        }

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
