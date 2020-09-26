using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Obsolete("Use LookAtCamera instead", true)]
public class Billboard : MonoBehaviour {
    private Transform _cam;

    private void Start() {
        _cam = Camera.main.transform;
    }

    private void Update() {
        transform.forward = _cam.forward;
    }
}