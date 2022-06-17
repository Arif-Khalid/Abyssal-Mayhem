using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public static Transform playerSpawn;
    // Start is called before the first frame update
    void Start()
    {
        playerSpawn = this.transform;
    }
}
