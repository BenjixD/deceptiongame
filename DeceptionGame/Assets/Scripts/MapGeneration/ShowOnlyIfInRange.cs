 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnlyIfInRange : MonoBehaviour
{
    public float showRangeSquared = 200; // TODO: This should be a universal value in PlayerController or somewhere
    public bool drawDebugLine = false;

    private Transform target;

    private Transform[] allChildren;
    private List<int> childLayers = new List<int>();

    private int notInRangeLayer;

    private bool isInvisible = false;


    void Awake()
    {
        this.notInRangeLayer = LayerMask.NameToLayer("Invisible");

        this.allChildren = this.GetComponentsInChildren<Transform>();
        for (int i = 0; i < this.allChildren.Length; i++) {
            childLayers.Add(this.allChildren[i].gameObject.layer);
        }
    }


    void Start() {
        this.target = GameManager.Instance.controller.mainPlayer.transform;
        if (this.target == null) {
            Debug.LogError("ERROR: No Main player found!");
            return;
        }

        if (isWithinDistance()) {
            this.OnGainVision();
            this.OnVisible();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target) {
            if (drawDebugLine) {
                Vector3 from = transform.position;
                Vector3 to = transform.position + Vector3.right * Mathf.Sqrt(showRangeSquared);
                Debug.DrawLine(from, to, Color.yellow);
            }

            bool withinDistance = this.isWithinDistance();

            if (withinDistance) {
                if (isInvisible) {
                    this.OnGainVision();
                }

                isInvisible = false;
                this.OnVisible();
            } else {
                if (!isInvisible) {
                    this.OnLostVision();
                }

                isInvisible = true;
                this.OnNoVision();
            }

        }
    }

    protected bool isWithinDistance() {
        float deltaX = target.transform.position.x - this.transform.position.x;
        float deltaZ = target.transform.position.z - this.transform.position.z;

        float distSquared = deltaX * deltaX + deltaZ * deltaZ;

        return distSquared <= this.showRangeSquared;
    }

    // Override me to do cool stuff!
    protected virtual void OnGainVision() {
        for (int i = 0; i < this.allChildren.Length; i++) {
            this.allChildren[i].gameObject.layer = this.childLayers[i];
        }
    }

    protected virtual void OnVisible() {

    }

    protected virtual void OnLostVision() {
        for (int i = 0; i < this.allChildren.Length; i++) {
            this.allChildren[i].gameObject.layer = this.notInRangeLayer;
        }
    }

    protected virtual void OnNoVision() {

    }
}
