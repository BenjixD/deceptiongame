using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple fog of war implementation from https://www.youtube.com/watch?v=iGAdaZ1ICaI

public enum Visibility {
    INVISIBLE = 0,
    INVISIBLE_BUT_SEEN_BEFORE = 1,
    VISIBLE = 2,

};

public class MapVisibility {
    public float alpha = 1.0f;
    public Visibility visibility;

    public MapVisibility(float alpha, Visibility visibility) {
        this.alpha = alpha;
        this.visibility = visibility;
    }

    public void Reset() {
        this.alpha = 1.0f;
        this.visibility = Visibility.INVISIBLE;
    }
};

public class MapVision {
    public Vector3 location;
    public float radius = -1; // -1 = player's radius
    public MapVision(Vector3 location, float radius = -1) {
        this.location = location;
        this.radius = radius;
    }
};

public class FogOfWarGenerator : MonoBehaviour {
	
	public GameObject fogOfWarPlane;
	public Transform player{get; set;}
	public LayerMask fogLayer;
	public float playerVisionRadius = 5f;

    public float unseenAlpha = 0.8f;

    public float maxDistance = 100;

	
	public Mesh m_mesh{get; set;}
	private Vector3[] m_vertices;
	private Color[] m_colors;
	
    private HashSet<int> seenIndices = new HashSet<int>();

    private MapController mapController;

    private MapVisibility[] playerMapVisibility;
    private Vector3[] worldPosVertices;

    private List<MapVision> visionLocations = new List<MapVision>();

    public void Initialize(MapController controller) {
        this.mapController = controller;
        this.fogOfWarPlane.SetActive(true); // Useful for debugging purposes

		Initialize();
    }
	
	// Update is called once per frame
	void Update () {
        if (player == null) {
            return;
        }

        MapVision playerVision = new MapVision(player.transform.position, this.playerVisionRadius);
        this.CastVisionOn(playerVision);

        this.UpdateMapVisibility();

        this.visionLocations.Clear();

	}

    // Casts vision FOR A SINGLE FRAME
    public void CastVisionOn(MapVision mapVision) {
        if (mapVision.radius == -1) {
            mapVision.radius = this.playerVisionRadius;
        }

        this.visionLocations.Add(mapVision);
    }

    // Cast vision using player's vision as reference
    public void CastVisionOn(Vector3 visionLocation) {
        this.CastVisionOn(new MapVision(visionLocation));
    }

    // This is where the magic happens for Fog of war
    private void UpdateMapVisibility() {

        for (int i=0; i< m_vertices.Length; i++) {
            this.playerMapVisibility[i].Reset();
        }

        foreach (MapVision mapVision in this.visionLocations) {
            Vector3 position = mapVision.location;
            Vector3 rayOrigin = position + Vector3.up * maxDistance;

            Ray r = new Ray(rayOrigin, position - rayOrigin);
            RaycastHit hit;

            Debug.DrawLine(rayOrigin, position, Color.red);
            if (Physics.Raycast(r, out hit, 1000, fogLayer, QueryTriggerInteraction.Collide)) {
                for (int i=0; i< m_vertices.Length; i++) {
                    Vector3 v = worldPosVertices[i];
                    MapGenerator.Coord coord = this.mapController.mapGenerator.NearestWorldPointToCoord(v);
            

                    float dist = Vector3.SqrMagnitude(v - hit.point);
                    float radiusSqr = mapVision.radius * mapVision.radius;

                    if (dist < radiusSqr) {
                        float attenuation = dist / radiusSqr;
                        
                        //float alpha = Mathf.Min(m_colors[i].a, dist/m_radiusSqr);
                        float alpha;
                        if (dist >= radiusSqr / 3) {
                            alpha = attenuation;
                        } else {
                            alpha = attenuation * attenuation;
                        }

                        alpha = Mathf.Min(unseenAlpha, alpha);
                        
                        //alpha = 0;
                        m_colors[i].a = alpha;
                        seenIndices.Add(i);
                        this.UpdateVisibilityIfBetter(this.playerMapVisibility, i, Visibility.VISIBLE, alpha);
                    } else {
                        if (seenIndices.Contains(i)) {
                            this.UpdateVisibilityIfBetter(this.playerMapVisibility, i,  Visibility.INVISIBLE_BUT_SEEN_BEFORE, unseenAlpha);
                        } else {
                            this.UpdateVisibilityIfBetter(this.playerMapVisibility, i,  Visibility.INVISIBLE, 1.0f);
                        }
                    }
                }
            }
        }

        UpdateColor();

    }

    private void UpdateVisibilityIfBetter(MapVisibility[] map, int index, Visibility visibility, float alpha) {
        if ( alpha < map[index].alpha ) {
            map[index].alpha = alpha;
            map[index].visibility = visibility;
        }
    }

	
	public void Initialize() {
		m_mesh = fogOfWarPlane.GetComponent<MeshFilter>().mesh;
		m_vertices = m_mesh.vertices;
		m_colors = new Color[m_vertices.Length];
        this.worldPosVertices = new Vector3[m_vertices.Length];

        playerMapVisibility = new MapVisibility[m_vertices.Length];

        for (int i=0; i< m_vertices.Length; i++) {
            this.worldPosVertices[i] = fogOfWarPlane.transform.TransformPoint(m_vertices[i]);
            this.playerMapVisibility[i] = new MapVisibility(1.0f, Visibility.INVISIBLE);
        }


		for (int i=0; i < m_colors.Length; i++) {
			m_colors[i] = Color.black;
		}
		UpdateColor();
	}

    public bool IsObjectVisible(Vector3 position) {

        int closestVertexIndex = 0;
        float closestSquaredDistance = float.MaxValue;
        for (int i = 0; i < m_vertices.Length; i++) {
            float distance = Vector3.SqrMagnitude(position - this.worldPosVertices[i]);
            if (distance < closestSquaredDistance) {
                closestVertexIndex = i;
                closestSquaredDistance = distance;
            }
        }

        return this.playerMapVisibility[closestVertexIndex].visibility == Visibility.VISIBLE;
    }
	
	void UpdateColor() {
        for (int i = 0; i < this.playerMapVisibility.Length; i++) {
            m_colors[i].a = this.playerMapVisibility[i].alpha;
        }

		m_mesh.colors = m_colors;
	}
}