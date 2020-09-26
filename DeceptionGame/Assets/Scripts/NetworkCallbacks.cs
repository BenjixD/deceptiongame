using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

[BoltGlobalBehaviour]
public class NetworkCallbacks : GlobalEventListener
{
    public override void SceneLoadLocalDone(string scene)
    {
        BoltNetwork.Instantiate(BoltPrefabs.Player, new Vector3(0,1,0), Quaternion.identity);
    }
}
