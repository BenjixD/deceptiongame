using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Bolt.Matchmaking;

public class StartNetworkConnection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
    BoltLauncher.StartServer();
    Debug.Log("Starting Server");
#else
    BoltLauncher.StartClient();
    Debug.Log("Starting Client");
#endif
    }
}
