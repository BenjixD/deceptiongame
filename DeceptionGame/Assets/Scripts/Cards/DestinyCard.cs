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

        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo)) {

            MapTileType[,] map = mapController.mapGenerator.GetMap();

            Vector3 teleportLocation = Vector3.zero;
            if (hitInfo.transform.tag == "Minimap") {
                // Clicked on minimap
                teleportLocation = mapController.minimap.MinimapToWorldPos(hitInfo.point);

            } else {
                teleportLocation = hitInfo.point;
            }

            MapGenerator.Coord nearestCoord = mapController.mapGenerator.NearestWorldPointToCoord(teleportLocation);
            

            if (nearestCoord.tileX == -1 || map[nearestCoord.tileX, nearestCoord.tileY] == MapTileType.WALL) {
                return false;
            }


            Vector3 finalTeleportPoint =  mapController.mapGenerator.CoordToWorldPoint(nearestCoord);

            if (!mapController.fogOfWarGenerator.IsObjectVisible(finalTeleportPoint)) {
                return false;
            }


            this.player.transform.position = finalTeleportPoint;
        }

        return true;
    }

}

