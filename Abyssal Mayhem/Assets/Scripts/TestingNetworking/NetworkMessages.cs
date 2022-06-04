using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class NetworkMessages : NetworkBehaviour
{
    [Command]
    public void CmdGetScene()
    {
        RpcSendScene(SceneManager.GetActiveScene().name);
        Debug.Log("Sending scene to client");
    }

    [ClientRpc]
    private void RpcSendScene(string scene)
    {
        SceneManager.LoadScene(scene);
        Debug.Log("Receiving scene from server");
    }
}
