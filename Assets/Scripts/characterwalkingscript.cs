using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class characterwalkingscript : MonoBehaviour
{

    public NavMeshAgent playerAgent;
    public Camera playerCamera;

    private Animator zhibAnimator;

    private bool activeAbility;

    // Start is called before the first frame update
    void Start()
    {
        zhibAnimator = GetComponent<Animator>();
        playerAgent.stoppingDistance = 2;
        activeAbility = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3))
            activeAbility = !activeAbility;

        if(!activeAbility)
        {
            if (Input.GetMouseButton(0))
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
        }
    }
}
