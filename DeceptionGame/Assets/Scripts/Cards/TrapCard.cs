using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TrapCard", order = 2)]
public class TrapCard : Card
{

    public TriggerOnCollisionObject prefabObject;

    public float damage;
    public float stunDuration;

    private TriggerOnCollisionObject instantiatedObject;

    protected override void PlayCardStart() {
        instantiatedObject = Instantiate<TriggerOnCollisionObject>(prefabObject);
        instantiatedObject.transform.position = this.player.transform.position;
        instantiatedObject.transform.SetParent(null);

        TriggerOnCollisionObject.InitializeDamageStunVelocityHelper(instantiatedObject, this.player, damage, stunDuration, Vector3.zero );

        this.RemoveCardFromActiveList();
    }
}
