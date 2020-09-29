using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnDamageObjectCard", order = 2)]
public class SpawnDamageObjectCard : Card
{
    public float damage = 1;
    public float speed = 20;
    public TriggerOnCollisionObject prefabObject;
    protected override void PlayCardStart() {
        base.PlayCardStart();

        // Not sure if we would want more refined logic to do this...
        Transform spawnLocation = this.player.throwLocation;
        Transform parentObject = null;

        Vector3 velocity = this.GetDirectionHelper() * speed;

        TriggerOnCollisionObject newObject = Instantiate<TriggerOnCollisionObject>(prefabObject);
        TriggerOnCollisionObject.InitializeDamageStunVelocityHelper(newObject, this.player, this.damage, 0, velocity);

        newObject.transform.position = spawnLocation.transform.position;
        newObject.transform.SetParent(parentObject);

        this.RemoveCardFromActiveList();
    }

}
