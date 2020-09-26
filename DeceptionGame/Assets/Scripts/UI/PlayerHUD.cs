using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {
    public Image propImage;
    public GameObject dropButton;

    private void Start() {
        EmptyProp();
    }

    public void PickUpProp(PhysicalProp prop) {
        propImage.enabled = true;
        propImage.sprite = prop.sprite;
        dropButton.SetActive(true);
    }

    public void EmptyProp() {
        propImage.enabled = false;
        dropButton.SetActive(false);
    }
}
