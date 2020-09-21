using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ShowHideWithMinimap : ShowOnlyIfInRange
{

    public bool removeFromMinimapOnLostVision = false; // Should only be true for players to avoid confusion
    public Sprite minimapSprite;


    protected override void OnGainVision() {
        base.OnGainVision();
        MinimapMarkerParams mapParams = new MinimapMarkerParams(minimapSprite, this.transform, this.gameObject.GetHashCode());
        GameManager.Instance.controller.mapController.minimap.AddToMinimap(mapParams);
    }

    protected override void OnLostVision() {
        base.OnLostVision();
        GameManager.Instance.controller.mapController.minimap.RemoveFromMinimap(this.gameObject.GetHashCode(), this.removeFromMinimapOnLostVision);
    }

}
