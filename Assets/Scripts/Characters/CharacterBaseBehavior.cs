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
    private Material materialHolder;
    private float elapse_time;

    public int ultimateCost;
    public LayerMask whatIsSpice;
    [HideInInspector] public int playerSpice;
    private int spiceTotal;
    private bool notAvailable;

    [Header("- Movement Speeds -")]
    public float walkMovementSpeed;
    public float crouchMovementSpeed;
    public float runMovementSpeed;
    private float movementSpeed;

    [Header("- Sound Ranges -")]
    public float walkSoundRange;
    public float crouchSoundRange;
    public float runSoundRange;

    [Header("- Sound Multipliers -")]
    public float crouchMultiplier;
    private bool crouching = false;
    public float runMultiplier;
    private bool running = false;
    [HideInInspector] public float detectionMultiplier;

    [HideInInspector] public bool invisible;

    [HideInInspector] public bool abilityActive;
    [HideInInspector] public bool ability1Active;
    [HideInInspector] public bool ability2Active;
    [HideInInspector] public bool ability3Active;

    private Animator animator;
    [HideInInspector] public PlayerState state = PlayerState.IDLE;

    private Vector3 targetPosition;
    private float timer;

    private float main_time;
    private float click_time = 0.333f;
    private bool clickUp = true;
    private int count;

    ///////////////////////////////////////////////
    // Cosa de empezar con q vayan los enemies
    public bool startInvincible;
    private float startInvincibleTime = 5f;
    private float startInvincibleTimer = 0f;
    private int initHealth;
    ///////////////////////////////////////////////

    // Start is called before the first frame update
    void Start()
    {
        playerAgent = GetComponent<NavMeshAgent>();
        playerAgent.speed = walkMovementSpeed;

        playerCamera = Camera.main;

        invisible = false;

        allSelected = false;
        hunterSeeking = false;
        animator = GetComponent<Animator>();
        playerAgent.stoppingDistance = 2;
        abilityActive = false;

        detectionMultiplier = 1.0f;

        child = transform.Find(name + "_low");
        materialHolder = child.gameObject.GetComponent<Renderer>().material;
        elapse_time = 0;
        playerSpice = 0;

        notAvailable = false;

        initHealth = playerHealth;

        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(startInvincible)
        {
            playerHealth = 99;
            while (startInvincibleTimer < startInvincibleTime)
            {
                startInvincibleTimer += Time.deltaTime;
                return;
            }

            playerHealth = initHealth;
            startInvincible = false;
        }


        spiceTotal = GameObject.Find("playercamera").GetComponent<GeneralManager>().totalSpice;

        if (hit) PlayerHit();

        if (allSelected)
        {
            MovementBehaviour();
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

            if(abilityActive && Input.GetMouseButtonDown(1))
            {
                ability1Active = false;
                ability2Active = false;
                ability3Active = false;
                abilityActive = false;
                Destroy(gameObject.GetComponent<LineRenderer>());
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) && !ability3Active && !ability2Active)
                if(gameObject.GetComponent<LineRenderer>() != null)
                    Destroy(gameObject.GetComponent<LineRenderer>());

            if (Input.GetKeyDown(KeyCode.Alpha2) && !ability1Active && !ability3Active)
                if (gameObject.GetComponent<LineRenderer>() != null)
                    Destroy(gameObject.GetComponent<LineRenderer>());

            if (Input.GetKeyDown(KeyCode.Alpha3) && !ability1Active && !ability2Active)
                if (gameObject.GetComponent<LineRenderer>() != null)
                    Destroy(gameObject.GetComponent<LineRenderer>());

            if (Input.GetKeyDown(KeyCode.LeftShift)/* && state != PlayerState.IDLE*/)
            {
                crouching = !crouching;
                running = false;
                if(crouching) {
                    detectionMultiplier = crouchMultiplier;
                    state = PlayerState.CROUCH; 
                }
                else if (state == PlayerState.CROUCH) {
                    if(targetPosition != Vector3.zero)
                    {
                        if (CalculateAbsoluteDistance(targetPosition).magnitude > playerAgent.stoppingDistance)
                        {
                            detectionMultiplier = 1f;
                            state = PlayerState.WALKING; 
                        } 
                    } else
                    {
                        if (running) {
                            detectionMultiplier = runMultiplier;
                            state = PlayerState.RUNNING; }
                        else
                        {
                            detectionMultiplier = 1f;
                            state = PlayerState.WALKING;
                        }
                    }
                }
            }

            if (!abilityActive)
            {
                MovementBehaviour();
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
                    else if (running) { state = PlayerState.RUNNING; }
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

        if (state==PlayerState.CROUCH) { movementSpeed = crouchMovementSpeed; }
        else if (state == PlayerState.RUNNING) { movementSpeed = runMovementSpeed; }
        else { movementSpeed = walkMovementSpeed; }

        playerAgent.speed = movementSpeed;
    }
    void OnGUI()
    {
        if (selectedCharacter)
        {
            GUI.Box(new Rect(5, 5, 50, 25), "Health");
            GUI.Box(new Rect(62, 5, 30, 25), playerHealth.ToString());
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

    void MovementBehaviour()
    {
        if (Input.GetMouseButton(1))
        {

            if (main_time == 0.0f)
            {
                main_time = Time.time;
            }

            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit meshHit;

            if (Physics.Raycast(ray, out meshHit))
            {
                if (meshHit.collider.tag == "Floor")
                {
                    if(clickUp) count++;

                    if (Time.time - main_time < click_time)
                    {
                        if (count == 2)
                        {
                            running = true;
                            detectionMultiplier = runMultiplier;
                            main_time = Time.time;
                        }

                        if(count > 2)
                        {
                            main_time = Time.time;
                            count = 2;
                        }
                    } else
                    {
                        running = false;
                        count = 0;
                        main_time = 0;
                    }

                    playerAgent.SetDestination(meshHit.point);
                    targetPosition = meshHit.point;

                    if (crouching) { state = PlayerState.CROUCH; }
                    else if (running) { state = PlayerState.RUNNING; }
                    else {
                        detectionMultiplier = 1f; 
                        state = PlayerState.WALKING; }
                }
            }

            clickUp = false;
        }

        if(playerAgent.remainingDistance == 0 && !playerAgent.pathPending)
        {
            running = false;
            count = 0;
            main_time = 0;
            detectionMultiplier = 1f;
            state = PlayerState.IDLE;
        }

        if(Input.GetMouseButtonUp(1))
        {

            clickUp = true;
        }

        if(Time.time - main_time > click_time)
        {
            running = false;
            count = 0;
            main_time = 0;
        }
    }
}
