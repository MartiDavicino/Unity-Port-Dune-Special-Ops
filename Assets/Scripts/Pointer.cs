using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public characterwalkingscript info;
    public Transform pos;
    Vector3 spawn = new Vector3(0,-12,0);

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (info.playerAgent.transform.position != info.playerAgent.destination)
            pos.position = info.playerAgent.destination;
        else if (info.playerAgent.transform.position == info.playerAgent.destination)
            pos.position = spawn;
    }
}
