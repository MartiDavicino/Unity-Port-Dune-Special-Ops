using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class characterwalkingscript : MonoBehaviour
{

    public NavMeshAgent playerAgent;
    public Camera playerCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {

            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit meshHit;

            if( Physics.Raycast(ray,out meshHit))
            {
                if(meshHit.collider.tag == "Floor")
                {
                    playerAgent.SetDestination(meshHit.point);
                }
            }
        }
        
    }
}
