using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class characterwalkingscript : MonoBehaviour
{

    public NavMeshAgent playerAgent;
    public Camera playerCamera;

    private Animator zhibAnimator;

    // Start is called before the first frame update
    void Start()
    {
        zhibAnimator = GetComponent<Animator>();
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
                    
                    if (zhibAnimator != null)
                    {
                        zhibAnimator.SetTrigger("isWalking");
                    }
                }
            }
        }
        
        if(playerAgent.remainingDistance <= playerAgent.stoppingDistance)
        {
            if (zhibAnimator != null)
            {
                zhibAnimator.SetTrigger("hasStopped");
            }
        }


        //if(playerAgent.velocity != Vector3.zero)
        //{
        //    if(zhibAnimator != null)
        //    {
        //        zhibAnimator.SetTrigger("isWalking");
        //    }
        //} else if (playerAgent.velocity == Vector3.zero)
        //{
        //}
    }
}
