using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DestinyCard", order = 2)]
public class DestinyCard : Card
{

    public float visionTime = 3.0f;
    protected override void PlayCardStart() {
        base.PlayCardStart();

        GameManager.Instance.controller.StartCoroutine(DestinyEffect());
    }

    private IEnumerator DestinyEffect() {


        List<PlayerController> players = GameManager.Instance.controller.mapController.players;

        float counter = 0;

        while (counter < visionTime) {
            foreach (PlayerController player in players) {
                if (this.player != player) {
                    GameManager.Instance.controller.fogOfWarGenerator.CastVisionOn(player.transform.position);
                }
            }

            // TODO: Probably want to centralize this somewhere later...
            if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject()) {
                if (this.DoTeleport()) {
                    counter = visionTime;
                }
            }

            counter += Time.deltaTime;

            yield return null;
        }


        this.RemoveCardFromActiveList();

    }

    private bool DoTeleport() {
        MapController mapController = GameManager.Instance.controller.mapController; // Just to shorten things

        Vector3 finalTeleportPoint = mapController.GetMapOrMinimapClickLocation(false);

        if (finalTeleportPoint.z == -1) {
            return false;
        }

        if (!mapController.fogOfWarGenerator.IsObjectVisible(finalTeleportPoint)) {
            return false;
        }

        this.player.transform.position = finalTeleportPoint;

        return true;
    }

}

