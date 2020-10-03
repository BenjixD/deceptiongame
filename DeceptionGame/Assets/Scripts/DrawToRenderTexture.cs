using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawToRenderTexture : MonoBehaviour
{

    public RawImage renderTarget;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = this.GetComponent<Camera>();
        int width = Screen.width;
        int height = Screen.height;
        RenderTexture rt = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
        rt.Create();

        cam.targetTexture = rt;

        renderTarget.texture = rt;

        RectTransform renderTargetTransform = renderTarget.GetComponent<RectTransform>();
        renderTargetTransform.sizeDelta = new Vector2(width, height);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
