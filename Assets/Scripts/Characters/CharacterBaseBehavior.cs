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
    STEALTH_KILL,
    ABILITY1,
    ABILITY2,
    ABILITY3,
    ABILITY3_1,
    NONE
}

public class CharacterBaseBehavior : MonoBehaviour
{

    public bool selectedCharacter;

    public NavMeshAgent playerAgent;
    public Camera playerCamera;

    public int playerHealth;

    private Animator animator;
    public bool ability1Active;
    public bool ability2Active;
    public bool ability3Active;
    public PlayerState state = PlayerState.IDLE;
    public bool abilityActive;

    private bool allSelected;

    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        allSelected = false;
        animator = GetComponent<Animator>();
        playerAgent.stoppingDistance = 2;
        abilityActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyDown(KeyCode.C))
            allSelected = !allSelected;

        if (allSelected && playerHealth != 0)
        {
            if (Input.GetMouseButton(0))
            {

                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit meshHit;

                if (Physics.Raycast(ray, out meshHit))
                {
                    if (meshHit.collider.tag == "Floor")
                    {
                        playerAgent.SetDestination(meshHit.point);
                        targetPosition = meshHit.point;

                        state = PlayerState.WALKING;

                    }
                    else
                    {
                        state = PlayerState.WALKING;
                        targetPosition = meshHit.point;
                    }
                }
            }
        }

        if (selectedCharacter && !allSelected && playerHealth != 0)
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

                            state = PlayerState.WALKING;
                            
                        } else
                        {
                            state = PlayerState.WALKING;
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

    void OnGUI()
    {
        if (selectedCharacter)
        {
            if (playerHealth != 0)
            {
            
                    GUI.Box(new Rect(5, 5, 50, 25), "Health");
                    GUI.Box(new Rect(62, 5, 20, 25), playerHealth.ToString());
            } else
            {
                GUI.Box(new Rect(Screen.width /2, (Screen.height/2) - 25, 150, 25), "This Character Died");
            }
        }

        if (allSelected)
            GUI.Box(new Rect(0, Screen.height - 25, 150, 25), "Moving Both Characters");

    }
    Vector3 CalculateAbsoluteDistance(Vector3 targetPos)
    {
        Vector3 distance = new Vector3(0f, 0f, 0f);

        distance.x = Mathf.Abs(transform.position.x - targetPos.x);
        distance.z = Mathf.Abs(transform.position.z - targetPos.z);

        return distance;
    }
}
