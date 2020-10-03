using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveVisionToMainPlayer : MonoBehaviour
{

    public float radius = -1; // -1 = the player's default vision
    private MapVision vision;
    void Start() {
        vision = new MapVision(this.transform.position, this.radius);
    }

    // Update is called once per frame
    void Update()
    {
        this.vision.location = this.transform.position;
        GameManager.Instance.controller.fogOfWarGenerator.CastVisionOn(vision);
    }
}
