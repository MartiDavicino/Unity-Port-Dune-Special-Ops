using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

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

    [HideInInspector] public bool selectedCharacter;
    [HideInInspector] public bool allSelected;

    private NavMeshAgent playerAgent;
    private Camera playerCamera;

    public int playerHealth;
    public float movementSpeed;

    [HideInInspector] public bool invisible;

    [HideInInspector] public bool abilityActive;
    [HideInInspector] public bool ability1Active;
    [HideInInspector] public bool ability2Active;
    [HideInInspector] public bool ability3Active;

    private Animator animator;
    [HideInInspector] public PlayerState state = PlayerState.IDLE;

    private Vector3 targetPosition;

    private bool crouching = false;

    // Start is called before the first frame update
    void Start()
    {
        playerAgent = GetComponent<NavMeshAgent>();
        playerAgent.speed = movementSpeed;

        playerCamera = Camera.main;

        invisible = false;

        allSelected = false;
        animator = GetComponent<Animator>();
        playerAgent.stoppingDistance = 2;
        abilityActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (allSelected)
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

        if (selectedCharacter && !allSelected)
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

            if (Input.GetKeyDown(KeyCode.LeftShift)/* && state != PlayerState.IDLE*/)
            {
                crouching = !crouching;
                if(crouching && state == PlayerState.IDLE) { state = PlayerState.CROUCH; }
                else if (state == PlayerState.CROUCH) { state = PlayerState.IDLE; }
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
                            targetPosition = meshHit.point;

                            if (crouching) { state = PlayerState.CROUCH; }
                            else { state = PlayerState.WALKING; }

                        } else
                        {
                            if (crouching) { state = PlayerState.CROUCH; }
                            else { state = PlayerState.WALKING; }
                            
                            targetPosition = meshHit.point;
                        }
                    }
                }

                
                
                //if (Input.GetKeyUp(KeyCode.LeftShift) && state != PlayerState.IDLE)
                //{
                //    state = PlayerState.WALKING;
                //}
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
                    if (crouching) { state = PlayerState.CROUCH; }
                    else { state = PlayerState.IDLE; }
                    playerAgent.ResetPath();
                }
            }
        }
    }

    void LateUpdate()
    {
        invisible = false;

        if (state==PlayerState.CROUCH) { movementSpeed = 5.0f; }
        else { movementSpeed = 7.5f; }

        playerAgent.speed = movementSpeed;
    }
    void OnGUI()
    {
        if (selectedCharacter)
        {
            GUI.Box(new Rect(5, 5, 50, 25), "Health");
            GUI.Box(new Rect(62, 5, 20, 25), playerHealth.ToString());
        }

        if (allSelected)
            GUI.Box(new Rect(5, Screen.height - 30, 150, 25), "Moving Both Characters");

    }
    Vector3 CalculateAbsoluteDistance(Vector3 targetPos)
    {
        Vector3 distance = new Vector3(0f, 0f, 0f);

        distance.x = Mathf.Abs(transform.position.x - targetPos.x);
        distance.z = Mathf.Abs(transform.position.z - targetPos.z);

        return distance;
    }
}
