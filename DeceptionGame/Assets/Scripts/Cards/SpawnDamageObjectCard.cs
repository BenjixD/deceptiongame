using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnDamageObjectCard", order = 2)]
public class SpawnDamageObjectCard : Card
{
    public float speed = 20;
    public DamageObject damageObject;
    protected override void PlayCardStart() {
        base.PlayCardStart();

        // Not sure if we would want more refined logic to do this...
        Transform spawnLocation = this.player.throwLocation;
        Transform parentObject = null;

        Vector3 velocity = Vector3.zero;

        PlayerHorizontalDirection horizontalDirection = this.player.GetHorizontalDirection();
        PlayerVerticalDirection verticalDirection = this.player.GetVerticalDirection();

        if (horizontalDirection != PlayerHorizontalDirection.DEFAULT) {
            velocity.x = this.player.GetHorizontalDirection() == PlayerHorizontalDirection.LEFT ? -speed : speed;
        }

        if (verticalDirection != PlayerVerticalDirection.DEFAULT) {
            velocity.z = this.player.GetVerticalDirection() == PlayerVerticalDirection.DOWN ? -speed : speed;
        }

        // If neutral, do right
        if (velocity.x == 0 && velocity.z == 0) {
            velocity.x = speed;
        }


        DamageObject dmgObject = Instantiate<DamageObject>(damageObject);
        dmgObject.transform.position = spawnLocation.transform.position;
        dmgObject.transform.SetParent(parentObject);
        dmgObject.Initialize(this.player, this, velocity);
    }

}
