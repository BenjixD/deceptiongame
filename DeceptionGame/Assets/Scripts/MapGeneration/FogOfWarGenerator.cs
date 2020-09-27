using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple fog of war implementation from https://www.youtube.com/watch?v=iGAdaZ1ICaI

public enum MapVisibility {
    INVISIBLE = 0,
    VISIBLE = 1,
    INVISIBLE_BUT_SEEN_BEFORE = 2
};

public class FogOfWarGenerator : MonoBehaviour {
	
	public GameObject fogOfWarPlane;
	public Transform player{get; set;}
	public LayerMask fogLayer;
	public float radius = 5f;

    public float unseenAlpha = 0.8f;

    public float maxDistance = 100;

	private float m_radiusSqr { get { return radius*radius; }}
	
	public Mesh m_mesh{get; set;}
	private Vector3[] m_vertices;
	private Color[] m_colors;
	

    private HashSet<int> seenIndices = new HashSet<int>();

    private MapController mapController;

    private MapVisibility[] playerMapVisibility;
    private Vector3[] worldPosVertices;

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

        this.CastVisionOn(player.transform.position);
	}

    public void CastVisionOn(Vector3 position) {

        // TODO: Fix bug
        Vector3 rayOrigin = position + Vector3.up * maxDistance;

		Ray r = new Ray(rayOrigin, position - rayOrigin);
		RaycastHit hit;

        Debug.DrawLine(rayOrigin, position, Color.red);
		if (Physics.Raycast(r, out hit, 1000, fogLayer, QueryTriggerInteraction.Collide)) {
			for (int i=0; i< m_vertices.Length; i++) {
				Vector3 v = worldPosVertices[i];
                MapGenerator.Coord coord = this.mapController.mapGenerator.NearestWorldPointToCoord(v);
        

				float dist = Vector3.SqrMagnitude(v - hit.point);
				if (dist < m_radiusSqr) {
                    float attenuation = dist / m_radiusSqr;
                    
					//float alpha = Mathf.Min(m_colors[i].a, dist/m_radiusSqr);
                    float alpha;
                    if (dist >= m_radiusSqr / 3) {
                        alpha = attenuation;
                    } else {
                        alpha = attenuation * attenuation;
                    }

                    alpha = Mathf.Min(unseenAlpha, alpha);
                     
                    //alpha = 0;
					m_colors[i].a = alpha;
                    seenIndices.Add(i);
                    this.playerMapVisibility[i] = MapVisibility.VISIBLE;
				} else {
                    if (seenIndices.Contains(i)) {
                        m_colors[i].a = unseenAlpha;
                        this.playerMapVisibility[i] = MapVisibility.INVISIBLE_BUT_SEEN_BEFORE;
                    } else {
                        this.playerMapVisibility[i] = MapVisibility.INVISIBLE;
                    }
                }
			}
			UpdateColor();
		}
    }

	
	public void Initialize() {
		m_mesh = fogOfWarPlane.GetComponent<MeshFilter>().mesh;
		m_vertices = m_mesh.vertices;
		m_colors = new Color[m_vertices.Length];
        this.worldPosVertices = new Vector3[m_vertices.Length];
        for (int i=0; i< m_vertices.Length; i++) {
            this.worldPosVertices[i] = fogOfWarPlane.transform.TransformPoint(m_vertices[i]);
        }

        playerMapVisibility = new MapVisibility[m_vertices.Length];

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

        return this.playerMapVisibility[closestVertexIndex] == MapVisibility.VISIBLE;
    }
	
	void UpdateColor() {
		m_mesh.colors = m_colors;
	}
}