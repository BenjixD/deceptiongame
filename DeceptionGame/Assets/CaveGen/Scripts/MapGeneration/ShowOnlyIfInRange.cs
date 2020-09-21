using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnlyIfInRange : MonoBehaviour
{
    public float showRangeSquared = 100;
    public bool drawDebugLine = false;

    private Transform target;

    private Transform[] allChildren;
    private List<int> childLayers = new List<int>();

    private int notInRangeLayer;

    private bool isInvisible = false;


    public void Initialize(Transform target) {
        this.target = target;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.notInRangeLayer = LayerMask.NameToLayer("Invisible");

        this.allChildren = this.GetComponentsInChildren<Transform>();
        for (int i = 0; i < this.allChildren.Length; i++) {
            childLayers.Add(this.allChildren[i].gameObject.layer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target) {
            float deltaX = target.transform.position.x - this.transform.position.x;
            float deltaZ = target.transform.position.z - this.transform.position.z;

            float distSquared = deltaX * deltaX + deltaZ * deltaZ;

            if (drawDebugLine) {
                Vector3 from = transform.position;
                Vector3 to = transform.position + Vector3.right * Mathf.Sqrt(showRangeSquared);
                Debug.DrawLine(from, to, Color.yellow);
            }

            if (distSquared > this.showRangeSquared) {
                if (!isInvisible) {
                    for (int i = 0; i < this.allChildren.Length; i++) {
                        this.allChildren[i].gameObject.layer = this.notInRangeLayer;
                    }
                }

                isInvisible = true;
            } else if (distSquared <= this.showRangeSquared) {
                if (isInvisible) {
                    for (int i = 0; i < this.allChildren.Length; i++) {
                        this.allChildren[i].gameObject.layer = this.childLayers[i];
                    }
                }

                isInvisible = false;
            }
        }
    }
}
