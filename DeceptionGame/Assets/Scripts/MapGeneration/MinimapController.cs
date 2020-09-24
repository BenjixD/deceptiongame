using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct MinimapMarkerParams {
    public Sprite sprite;
    public Transform transform;
    public int hashCode;
    public bool showOverFogOfWar;
    // TODO: Add size 

    public MinimapMarkerParams(Sprite sprite, Transform transform, int hashCode, bool showOverFogOfWar = false) {
        this.sprite = sprite;
        this.transform = transform;
        this.hashCode = hashCode;
        this.showOverFogOfWar = showOverFogOfWar;
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
    public Transform markerSeeThroughParent;

    public Image spritePrefab;



    private Dictionary<int, MinimapMarkerParams> minimapMarkers = new Dictionary<int, MinimapMarkerParams>();

    private MapController mapController;

    private PlayerController mainPlayer;


    // Initialize is AFTER we get our main character
    public void Initialize(MapController mapController, PlayerController mainPlayer) {
        this.mapController = mapController;
        this.mainPlayer = mainPlayer;

        Mesh mesh = new Mesh();
		
        minimapMeshFilter.mesh = mesh;


		mesh.vertices = this.mapController.mapGenerator.meshGenerator.vertices.ToArray();
		mesh.triangles = this.mapController.mapGenerator.meshGenerator.triangles.ToArray();
		mesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateMarker();
    }

    public void AddToMinimap(MinimapMarkerParams minimapParams) {

        Image newMarker = Instantiate<Image>(this.spritePrefab, markerParent);
        if (minimapParams.showOverFogOfWar) {
            newMarker.transform.SetParent(markerSeeThroughParent);
        }

        newMarker.sprite = minimapParams.sprite;
        minimapParams.markerObject = newMarker;
        this.PositionOnMinimap(minimapParams);

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

    public void MoveToMinimapLocation(Vector3 minimapRaycastPos) {
        Vector3 minimapToWorldPos = this.minimapMeshFilter.transform.InverseTransformPoint(minimapRaycastPos);
        minimapToWorldPos.y = -this.mapController.mapGenerator.meshGenerator.wallHeight;
        this.mapController.SetCameraTarget(minimapToWorldPos, null);
    }

    private void UpdateMarker() {
        List<int> markersToDestroy = new List<int>();

        foreach (KeyValuePair<int, MinimapMarkerParams> markers in this.minimapMarkers) {
            MinimapMarkerParams minimapParams = markers.Value;

            if (minimapParams.transform == null) {
                Debug.LogWarning("Warning: Minimap object destroyed but not removed!");
                markersToDestroy.Add(markers.Key);
                continue;
            }

            this.PositionOnMinimap(minimapParams);
        }

        foreach (int item in markersToDestroy) {
            Destroy(this.minimapMarkers[item].markerObject.gameObject);
            this.minimapMarkers.Remove(item);
        }
    }

    private void PositionOnMinimap(MinimapMarkerParams minimapParams) {
        Vector3 transformedPoint = minimapMeshFilter.transform.TransformPoint(minimapParams.transform.position + Vector3.up * forwardOffset);
        minimapParams.markerObject.transform.position = transformedPoint;
    }
}
