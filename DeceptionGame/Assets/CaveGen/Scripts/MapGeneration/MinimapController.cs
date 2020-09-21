using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{

    public MeshFilter minimapMeshFilter;
    public Image marker;

    public float forwardOffset = 1f;


    private MapGenerator mapGenerator;

    private GameObject mainPlayer;

    

    public void Initialize(MapGenerator mapGenerator, GameObject mainPlayer) {
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

    private void UpdateMarker() {
        Vector3 transformedPoint = minimapMeshFilter.transform.TransformPoint(mainPlayer.transform.position + Vector3.up * forwardOffset);
        this.marker.transform.position = transformedPoint;
    }
}
