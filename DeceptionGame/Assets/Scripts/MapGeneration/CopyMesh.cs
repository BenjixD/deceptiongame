using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyMesh : MonoBehaviour
{

    public MeshFilter myMeshFilter;

    public FogOfWarGenerator fogScript; // tmp

    // Start is called before the first frame update
    void Start()
    {
        myMeshFilter.sharedMesh = fogScript.m_mesh;
    }
}
