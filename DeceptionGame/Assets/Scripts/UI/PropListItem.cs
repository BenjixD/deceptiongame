using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropListItem : MonoBehaviour {
    public bool fulfilled;
    public Image image;

    public void SetImage(Sprite sprite) {
        image.sprite = sprite;
    }

    public void Check(bool sabotage) {
        // TODO: change temporary UI
        if (!sabotage) {
            image.color = Color.green;
        } else {
            // TODO: only show sabotage UI for red team
            image.color = Color.red;
        }

    }

    public void Uncheck() {
        // TODO: change temporary UI
        image.color = Color.white;
    }
}
