using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;

public class ClientConnector : GlobalEventListener
{
    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        if (!BoltNetwork.IsServer) {
            // Join The First Possible Room Created
            Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);

            foreach (var session in sessionList)
            {
                UdpSession photonSession = session.Value as UdpSession;

                if (photonSession.Source == UdpSessionSource.Photon)
                {
                    BoltMatchmaking.JoinSession(photonSession);
                }
            }
        }
    }
}
