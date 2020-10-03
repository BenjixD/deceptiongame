using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simply spawns an object.

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnObjectCard", order = 2)]
public class SpawnObjectCard : Card
{

    public GameObject prefabObject;
    public float duration = -1;

    private List<GameObject> instantiatedObjects = new List<GameObject>();

    protected override void PlayCardStart() {
        GameObject instantiatedObject = Instantiate<GameObject>(prefabObject);
        instantiatedObject.transform.position = this.player.transform.position;
        instantiatedObject.transform.SetParent(null);

        this.instantiatedObjects.Add(instantiatedObject);

        if (duration != -1) {
            DestroyAfterSeconds destroyAfterSecondsScript = instantiatedObject.GetComponent<DestroyAfterSeconds>();
            if (destroyAfterSecondsScript != null) {
                destroyAfterSecondsScript.time = duration;
            } else {
                Debug.LogWarning("Duration set, but no DestroyAfterSeconds script attached!");
            }
        }

        this.RemoveCardFromActiveList();
    }
}
