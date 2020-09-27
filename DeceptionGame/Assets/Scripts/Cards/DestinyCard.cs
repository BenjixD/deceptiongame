using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            //    if (this.player != player) {
                    GameManager.Instance.controller.fogOfWarGenerator.CastVisionOn(player.transform.position);
            //    }
            }

            counter += Time.deltaTime;

            yield return null;
        }


        this.RemoveCardFromActiveList();

    }
}
