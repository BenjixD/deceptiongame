using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour
{

    public CardUIView cardUIView;


    public void Initialize() {
        this.cardUIView.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
