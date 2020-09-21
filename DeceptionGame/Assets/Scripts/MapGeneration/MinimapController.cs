﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct MinimapMarkerParams {
    public Sprite sprite;
    public Transform transform;
    public int hashCode;
    // TODO: Add size 

    public MinimapMarkerParams(Sprite sprite, Transform transform, int hashCode) {
        this.sprite = sprite;
        this.transform = transform;
        this.hashCode = hashCode;
        this.markerObject = null;
    }

    public Image markerObject; // Set on use
};

public class MinimapController : MonoBehaviour
{

    public MeshFilter minimapMeshFilter;
    public Image marker;

    public float forwardOffset = 1f;

    public Transform markerParent;

    public Image spritePrefab;

    private Dictionary<int, MinimapMarkerParams> minimapMarkers = new Dictionary<int, MinimapMarkerParams>();

    private MapGenerator mapGenerator;

    private PlayerController mainPlayer;


    // Initialize is AFTER we get our main character
    public void Initialize(MapGenerator mapGenerator, PlayerController mainPlayer) {
        this.mapGenerator = mapGenerator;
        this.mainPlayer = mainPlayer;

        Mesh mesh = new Mesh();
		
        minimapMeshFilter.mesh = mesh;


		mesh.vertices = this.mapGenerator.meshGenerator.vertices.ToArray();
		mesh.triangles = this.mapGenerator.meshGenerator.triangles.ToArray();
		mesh.RecalculateNormals();
    }


    // Update is called once per frame
    void Update()
    {
        this.UpdateMarker();
    }

    public void AddToMinimap(MinimapMarkerParams minimapParams) {
        Image newMarker = Instantiate<Image>(spritePrefab, markerParent);
        newMarker.sprite = minimapParams.sprite;
        minimapParams.markerObject = newMarker;
        this.minimapMarkers.Add(minimapParams.hashCode, minimapParams);
    }

    public void RemoveFromMinimap(int hashCode, bool destroyMinimapObject) {
        if (!minimapMarkers.ContainsKey(hashCode)) {
            return;
        }
        
        MinimapMarkerParams minimapParams = minimapMarkers[hashCode];

        if (destroyMinimapObject) {
            Destroy(minimapParams.markerObject.gameObject);
        }
        
        this.minimapMarkers.Remove(hashCode);


    }

    private void UpdateMarker() {
        //Vector3 transformedPoint = minimapMeshFilter.transform.TransformPoint(mainPlayer.transform.position + Vector3.up * forwardOffset);
        //this.marker.transform.position = transformedPoint;

        List<int> markersToDestroy = new List<int>();

        foreach (KeyValuePair<int, MinimapMarkerParams> markers in this.minimapMarkers) {
            MinimapMarkerParams minimapParams = markers.Value;

            if (minimapParams.transform == null) {
                Debug.LogWarning("Warning: Minimap object destroyed but not removed!");
                markersToDestroy.Add(markers.Key);
                continue;
            }

            Vector3 transformedPoint = minimapMeshFilter.transform.TransformPoint(minimapParams.transform.position + Vector3.up * forwardOffset);
            minimapParams.markerObject.transform.position = transformedPoint;
        }

        foreach (int item in markersToDestroy) {
            Destroy(this.minimapMarkers[item].markerObject.gameObject);
            this.minimapMarkers.Remove(item);
        }
    }
}
