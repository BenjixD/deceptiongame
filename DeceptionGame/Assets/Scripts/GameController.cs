using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public MapController mapController;
    public PlayerController mainPlayer;

    // Start is called before the first frame update
    void Awake()
    {
        GameManager.Instance.controller = this;
        mapController.Initialize();
    }

    void Start() {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
