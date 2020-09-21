﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnDistance : MonoBehaviour
{
    public Camera cam;
    public float distToHide = 5f;

    public SpriteRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        if (cam == null) {
            cam = Camera.main;
        }
        
        this.HideMeIfTooFar();
    }

    // Update is called once per frame
    void Update()
    {
        this.HideMeIfTooFar();
    }

    void HideMeIfTooFar() {
        if (Vector3.Distance(cam.transform.position, this.transform.position) > distToHide) {
            rend.enabled = false;
        } else {
            rend.enabled = true;
        }
    }
}
