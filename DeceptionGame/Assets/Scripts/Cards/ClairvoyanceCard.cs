using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ClairvoyanceCard", order = 2)]
public class ClairvoyanceCard : Card
{
    public float scanDuration = 5f;
    public float scanRadius = 12f;
    protected override void PlayCardStart() {
        GameManager.Instance.controller.StartCoroutine(this.WaitForInput());
    }

    // TODO: More centralized way for input
    private IEnumerator WaitForInput() {

        while (true) {
            if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject()) {

                Vector3 location = GameManager.Instance.controller.mapController.GetMapOrMinimapClickLocation(true);
                if (location.z != -1) {
                    float counter = 0;
                    MapVision mapVision = new MapVision(location, this.scanRadius);

                    while (counter < this.scanDuration) {
                        counter += Time.deltaTime;
                        GameManager.Instance.controller.fogOfWarGenerator.CastVisionOn(mapVision);

                        yield return null;
                    }

                    yield break;
                }
            }

            yield return null;
        }
    }
}
