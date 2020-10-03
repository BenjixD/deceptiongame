using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProp", menuName = "Prop")]
public class Prop : ScriptableObject {
    public Sprite sprite;
    public Sprite destroyedSprite;
}
