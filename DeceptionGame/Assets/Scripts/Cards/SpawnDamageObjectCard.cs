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

        Vector3 velocity = this.GetDirectionHelper() * speed;

        DamageObject dmgObject = Instantiate<DamageObject>(damageObject);
        dmgObject.transform.position = spawnLocation.transform.position;
        dmgObject.transform.SetParent(parentObject);
        dmgObject.Initialize(velocity, this.player);

        this.RemoveCardFromActiveList();
    }

}
