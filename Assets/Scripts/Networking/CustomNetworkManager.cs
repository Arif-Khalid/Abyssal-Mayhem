using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    public override void OnClientConnect()
    {
        AudioManager.instance.StartCoroutine(AudioManager.instance.StartGameAudio());
        base.OnClientConnect();
    }

    public override void OnClientDisconnect()
    {
        AudioManager.instance.StopGameAudio();
        base.OnClientDisconnect();
    }
}
