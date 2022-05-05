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
    [HideInInspector] public bool hunterSeeking;

    private NavMeshAgent playerAgent;
    private Camera playerCamera;

    public int playerHealth;

    [HideInInspector] public bool hit;
    private Transform child;
    private Material currentMaterial;
    private Material materialHolder;
    private float elapse_time;

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

    public LayerMask whatIsSpice;

    [HideInInspector] public int playerSpice;

    private int spiceTotal;

    public int ultimateCost;

    private bool notAvailable;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        playerAgent = GetComponent<NavMeshAgent>();
        playerAgent.speed = movementSpeed;

        playerCamera = Camera.main;

        invisible = false;

        allSelected = false;
        hunterSeeking = false;
        animator = GetComponent<Animator>();
        playerAgent.stoppingDistance = 2;
        abilityActive = false;

        child = transform.Find(name + "_low");
        materialHolder = child.gameObject.GetComponent<Renderer>().material;
        elapse_time = 0;
        playerSpice = 0;

        notAvailable = false;

        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        spiceTotal = GameObject.Find("playercamera").GetComponent<GeneralManager>().totalSpice;

        if (hit) PlayerHit();

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

                if (spiceTotal < ultimateCost)
                {
                    timer = 3.0f;
                    notAvailable = true;
                }
                else
                {
                    ability3Active = !ability3Active;
                    abilityActive = !abilityActive;
                }       
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
            }

        } else
        {
            if(gameObject.GetComponent<LineRenderer>() != null && !hunterSeeking)
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

        Collider[] pickables = Physics.OverlapSphere(transform.position, 3.0f, whatIsSpice);

        for (int i = 0; i < pickables.Length; i++)
        {
            if (pickables[i].gameObject.tag == "SpiceSpot")
            {
                Destroy(pickables[i].gameObject);
                playerSpice += 1000;
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
        { GUI.Box(new Rect(5, Screen.height - 30, 150, 25), "Moving Both Characters"); }
        if (notAvailable)
        {
            if (timer <= 0f)
            {
                notAvailable = false;
            }
            else
            {
                timer -= Time.deltaTime;
                GUI.Box(new Rect(5, 75, 150, 25), "Not enough spice");
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

    void PlayerHit()
    {
        child.gameObject.GetComponent<Renderer>().material = Resources.Load(name + "hit", typeof(Material)) as Material;

        while (elapse_time < 0.5f)
        {
            elapse_time += Time.deltaTime;
            return;
        }

        elapse_time = 0;

        hit = false;
        child.gameObject.GetComponent<Renderer>().material = materialHolder;
    }
}
