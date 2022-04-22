using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PlayerState
{
    IDLE,
    WALKING,
    RUNNING,
    CROUCH,
    NONE
}

public class CharacterBaseBehavior : MonoBehaviour
{

    public bool selectedCharacter;

    public NavMeshAgent playerAgent;
    public Camera playerCamera;

    private Animator animator;

    public bool ability1Active;
    public bool ability2Active;
    public bool ability3Active;

    public PlayerState state = PlayerState.IDLE;

    public bool abilityActive;

    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
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
                            targetPosition = meshHit.point;

                            if (animator != null)
                            {
                                state = PlayerState.WALKING;
                            }
                        } else
                        {
                            targetPosition = meshHit.point;
                        }
                    }
                }
        

                if(Input.GetKey(KeyCode.LeftShift) && state != PlayerState.IDLE)
                {
                    state = PlayerState.CROUCH;
                }
                
                if (Input.GetKeyUp(KeyCode.LeftShift) && state != PlayerState.IDLE)
                {
                    state = PlayerState.WALKING;
                }
            }
        } else
        {
            if(gameObject.GetComponent<LineRenderer>() != null)
                Destroy(gameObject.GetComponent<LineRenderer>());
        }

        if(targetPosition != Vector3.zero)
        {
            if(CalculateAbsoluteDistance(targetPosition).magnitude <= playerAgent.stoppingDistance)
            {
                if (animator != null)
                {
                    targetPosition = Vector3.zero;
                    state = PlayerState.IDLE;
                    playerAgent.ResetPath();
                }
            }
        }
        

    }

    Vector3 CalculateAbsoluteDistance(Vector3 targetPos)
    {
        Vector3 distance = new Vector3(0f, 0f, 0f);

        distance.x = Mathf.Abs(transform.position.x - targetPos.x);
        distance.z = Mathf.Abs(transform.position.z - targetPos.z);

        return distance;
    }
}
