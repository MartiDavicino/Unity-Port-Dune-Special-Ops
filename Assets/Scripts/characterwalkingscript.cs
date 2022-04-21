using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PlayerState
{
    WALKING,
    RUNNING,
    CROUCH,
    NONE
}

public class characterwalkingscript : MonoBehaviour
{

    public bool selectedCharacter;

    public NavMeshAgent playerAgent;
    public Camera playerCamera;

    private Animator zhibAnimator;

    public bool ability1Active;
    public bool ability2Active;
    public bool ability3Active;

    public PlayerState state = PlayerState.WALKING;

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
        if (selectedCharacter)
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

            if (Input.GetKeyDown(KeyCode.Alpha1) && !ability3Active && !ability2Active)
                Destroy(gameObject.GetComponent<LineRenderer>());

            if (Input.GetKeyDown(KeyCode.Alpha2) && !ability1Active && !ability3Active)
                Destroy(gameObject.GetComponent<LineRenderer>());

            if (Input.GetKeyDown(KeyCode.Alpha3) && !ability1Active && !ability2Active)
                Destroy(gameObject.GetComponent<LineRenderer>());

            if(Input.GetKeyDown(KeyCode.LeftShift))
            {
                switch(state){
                    case  PlayerState.WALKING:
                        state = PlayerState.CROUCH;
                        break;
                    case PlayerState.CROUCH:
                        state = PlayerState.WALKING;
                        break;
                }
            }

            if (!abilityActive)
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
        } else
        {
            if(gameObject.GetComponent<LineRenderer>() != null)
                Destroy(gameObject.GetComponent<LineRenderer>());
        }
    }
}
