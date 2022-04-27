using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class NeralaPassive : MonoBehaviour
{
    private NavMeshAgent playerAgent;
    private Camera playerCamera;

    private CharacterBaseBehavior baseScript;

    private GameObject passiveCube;

    private Vector3 appearOffset;

    private bool goingTo;
    void Start()
    {
        playerAgent = GetComponent<NavMeshAgent>();
        playerCamera = Camera.main;

        appearOffset = new Vector3(0f, 0f, 2f);

        goingTo = false;
        baseScript = GetComponent<CharacterBaseBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if(baseScript.selectedCharacter)
        {
            if (Input.GetMouseButtonDown(0))
            {

                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit meshHit;

                if (Physics.Raycast(ray, out meshHit))
                {
                    if (meshHit.collider.tag == "NeralaPassive")
                    {
                        passiveCube = meshHit.collider.gameObject;

                        playerAgent.SetDestination(meshHit.point);
                        goingTo = true;

                    } else
                    {
                        goingTo = false;
                    }
                }
            }

            if(goingTo && playerAgent.remainingDistance < 1.5f && !playerAgent.pathPending)
            {

                appearOffset = new Vector3(253f, 8.5f, -67.57f);
                playerAgent.Warp(appearOffset);
                playerAgent.ResetPath();
                baseScript.state = PlayerState.IDLE;

                goingTo = false;
            }

        }
    }
}
