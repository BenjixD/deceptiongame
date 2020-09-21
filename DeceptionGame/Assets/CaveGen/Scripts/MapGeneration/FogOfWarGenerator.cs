using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple fog of war implementation from https://www.youtube.com/watch?v=iGAdaZ1ICaI

public class FogOfWarGenerator : MonoBehaviour {
	
	public GameObject fogOfWarPlane;
	public Transform player;
	public LayerMask fogLayer;
	public float radius = 5f;

    public float unseenAlpha = 0.8f;

    public float maxDistance = 100;

	private float m_radiusSqr { get { return radius*radius; }}
	
	public Mesh m_mesh{get; set;}
	private Vector3[] m_vertices;
	private Color[] m_colors;
	

    private HashSet<int> seenIndices = new HashSet<int>();

	// Use this for initialization
	void Start () {
		Initialize();
	}
	
	// Update is called once per frame
	void Update () {
        if (player == null) {
            return;
        }

		Ray r = new Ray(transform.position, player.position - transform.position);
		RaycastHit hit;

        Debug.DrawLine(transform.position, player.position, Color.red);
		if (Physics.Raycast(r, out hit, 1000, fogLayer, QueryTriggerInteraction.Collide)) {
			for (int i=0; i< m_vertices.Length; i++) {

				Vector3 v = fogOfWarPlane.transform.TransformPoint(m_vertices[i]);

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
				} else {
                    if (seenIndices.Contains(i)) {
                        m_colors[i].a = unseenAlpha;
                    }
                }
			}
			UpdateColor();
		}
	}

	
	void Initialize() {
		m_mesh = fogOfWarPlane.GetComponent<MeshFilter>().mesh;
		m_vertices = m_mesh.vertices;
		m_colors = new Color[m_vertices.Length];
		for (int i=0; i < m_colors.Length; i++) {
			m_colors[i] = Color.black;
		}
		UpdateColor();
	}
	
	void UpdateColor() {
		m_mesh.colors = m_colors;
	}
}