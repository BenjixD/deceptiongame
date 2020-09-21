using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CopySpriteBounds : ButtonScript
{
    public SpriteRenderer spriteRenderer;
    public BoxCollider collide;

    // Start is called before the first frame update
    public override void BuildObject()
    {
        if (spriteRenderer != null && collide != null) {
            collide.center = new Vector3(0, spriteRenderer.bounds.size.y/2f, 0);
            collide.size = spriteRenderer.bounds.size;
            Debug.Log("Copy sprite bounds to collider: " + spriteRenderer.bounds);
        }  
    }
}
