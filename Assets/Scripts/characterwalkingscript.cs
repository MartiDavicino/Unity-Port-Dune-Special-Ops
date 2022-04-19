using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class characterwalkingscript : MonoBehaviour
{

    public NavMeshAgent playerAgent;
    public Camera playerCamera;

    private Animator zhibAnimator;

    public bool ability1Active;
    public bool ability2Active;
    public bool ability3Active;

    public bool abilityActive;

    // Start is called before the first frame update
    void Start()
    {
        zhibAnimator = GetComponent<Animator>();
        playerAgent.stoppingDistance = 2;
        abilityActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && (!abilityActive || ability1Active))
        {
            ability1Active = !ability1Active;
            abilityActive = !abilityActive;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && (!abilityActive || ability2Active))
        {
            ability2Active = !ability2Active;
            abilityActive = !abilityActive;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && (!abilityActive || ability3Active))
        {
            ability3Active = !ability3Active;
            abilityActive = !abilityActive;
        }

        if(!abilityActive)
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
