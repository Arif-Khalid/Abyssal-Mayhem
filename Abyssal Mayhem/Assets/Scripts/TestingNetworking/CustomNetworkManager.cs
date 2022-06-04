using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    
    public override void OnStartServer()
    {
    }
    public override void OnClientConnect()
    {

        base.OnClientConnect();
    }

    
}
