using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;

public class ServerConnector : GlobalEventListener
{
    [SerializeField]
    private string _gameScene;
    
    public override void SceneLoadLocalDone(string scene)
    {
        if (BoltNetwork.IsServer)
        {
            string matchName = Guid.NewGuid().ToString();
            BoltMatchmaking.CreateSession(
                sessionID: matchName,
                sceneToLoad: _gameScene
            );
        }
    }
}
