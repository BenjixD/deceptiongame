using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WildCardsCard", order = 2)]
public class WildCardsCard : Card
{
    public float damage = 1f;
    public float speed = 20;
    public TriggerOnCollisionObject prefabObject;
    protected override void PlayCardStart() {
        base.PlayCardStart();

        // Not sure if we would want more refined logic to do this...
        Transform spawnLocation = this.player.throwLocation;
        Transform parentObject = null;

        Vector3 velocity = this.GetDirectionHelper() * speed;
        Vector3 velocity2 = Quaternion.AngleAxis(30, Vector3.up) * velocity;
        Vector3 velocity3 = Quaternion.AngleAxis(-30, Vector3.up) * velocity;

        this.CreateDamageObject(velocity, spawnLocation, parentObject);
        this.CreateDamageObject(velocity2, spawnLocation, parentObject);
        this.CreateDamageObject(velocity3, spawnLocation, parentObject);

        this.RemoveCardFromActiveList();
    }

    private void CreateDamageObject(Vector3 velocity, Transform spawnLocation, Transform parentObject) {
        TriggerOnCollisionObject newObject = Instantiate<TriggerOnCollisionObject>(prefabObject);

        TriggerOnCollisionObject.InitializeDamageStunVelocityHelper(newObject, this.player, this.damage, 0, velocity);

        newObject.transform.position = spawnLocation.transform.position;
        newObject.transform.SetParent(parentObject);
    }

}
