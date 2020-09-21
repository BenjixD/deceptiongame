using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {
    public Image propImage;

    private void Start() {
        EmptyPropImage();
    }

    public void EmptyPropImage() {
        propImage.enabled = false;
    }

    public void SetPropImage(Sprite image) {
        propImage.enabled = true;
        propImage.sprite = image;
    }
}
