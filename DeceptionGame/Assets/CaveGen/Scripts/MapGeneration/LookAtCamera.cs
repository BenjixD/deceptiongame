using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera camera;
    public bool doOnUpdate = true;
    public bool lookOpposite = false;

    public float maxSquaredDistance = 1000f;

    private Renderer rend;

    


    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();

        if (camera == null) {
            camera = Camera.main;
        }
        
        this.LookAtTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (doOnUpdate) {
            this.LookAtTarget();
        }
    }

    void LookAtTarget() {
        //if (!rend.isVisible) return;
        float deltaX = this.transform.position.x - this.camera.transform.position.x;
        float deltaY = this.transform.position.y - this.camera.transform.position.y;
        float squaredDistance = deltaX * deltaX + deltaY * deltaY;

        if ( squaredDistance >= maxSquaredDistance ) {
            return;
        }

        //if (rend != null && !IsVisible(this.gameObject, rend.bounds)) return;

        if (lookOpposite) {
            //transform.LookAt(2 * transform.position - camera.transform.position);
            transform.forward = -camera.transform.forward;
        } else {
            //transform.LookAt(camera.transform);
            transform.forward = camera.transform.forward;
        }
    }
}
