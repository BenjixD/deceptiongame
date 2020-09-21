using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMeshRendererSortingLayer : ButtonScript
{
    public List<MeshRenderer> renderers;
    
    public override void BuildObject() {
        int sortingLayerId = SortingLayer.NameToID("FogOfWar");

        foreach (MeshRenderer rend in renderers) {
            rend.sortingLayerID = sortingLayerId;
        }
    }

}
