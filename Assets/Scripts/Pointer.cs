using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    //public characterwalkingscript info;
    public GameObject player;
    public Transform pos;

    private UnityEngine.AI.NavMeshAgent playerAgent;

    Vector3 spawn = new Vector3(0,-12,0);

    void Start()
    {
        playerAgent = player.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(playerAgent.transform.position != playerAgent.destination)
            pos.position = playerAgent.destination;
        else if (playerAgent.transform.position == playerAgent.destination)
            pos.position = spawn;
            //if (info.playerAgent.transform.position != info.playerAgent.destination)
        //else if (info.playerAgent.transform.position == info.playerAgent.destination)
            //pos.position = spawn;
    }
}
