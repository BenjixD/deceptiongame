 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnlyIfInRange : MonoBehaviour
{
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

        if (GameManager.Instance.controller.fogOfWarGenerator.IsObjectVisible(this.transform.position)) {
            this.OnGainVision();
            this.OnVisible();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target) {
            bool withinDistance = GameManager.Instance.controller.fogOfWarGenerator.IsObjectVisible(this.transform.position); // TODO: Should use Bounds instead of just raw position
            
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
