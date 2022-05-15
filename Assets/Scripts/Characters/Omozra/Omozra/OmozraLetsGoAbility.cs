using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class OmozraLetsGoAbility : MonoBehaviour
{
    private GameObject sadiq;
    private SadiqBehaviour sadiqScript;

    [HideInInspector] public CharacterBaseBehavior baseScript;

    private NavMeshAgent agent;
    private Camera playerCamera;
    private RaycastHit rayHit;

    [HideInInspector] public bool addLineComponentOnce;
    [HideInInspector] public GameObject targetCharacter;
    [HideInInspector] public Vector3 targetPosition;

    [HideInInspector] public bool characterChosen;
    [HideInInspector] public bool locationChosen;

    private bool characterOutOfRange;

    [HideInInspector] public bool characterEaten;

    public float maximumRange;
    
    public float cooldown;
    [HideInInspector] public bool onCooldown;
    private float elapse_time;

    // Start is called before the first frame update
    void Start()
    {
        sadiq = GameObject.Find("Sadiq");
        sadiqScript = sadiq.GetComponent<SadiqBehaviour>();

        baseScript = gameObject.GetComponent<CharacterBaseBehavior>();
        playerCamera = Camera.main;

        addLineComponentOnce = true;

        characterChosen = false;
        locationChosen = false;

        characterOutOfRange = false;

        agent = GetComponent<NavMeshAgent>();

        onCooldown = false;
        elapse_time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(baseScript.selectedCharacter)
        {
            
            if (baseScript.ability3Active && !characterEaten)
            {
                if (addLineComponentOnce)
                {
                    addLineComponentOnce = false;
                    gameObject.AddComponent<LineRenderer>();
                }

                if (gameObject.GetComponent<LineRenderer>() != null)
                    gameObject.DrawCircleScaled(maximumRange, 0.05f, transform.localScale);

                if (Input.GetKeyDown(KeyCode.Mouse0) && !onCooldown && !characterChosen)
                {
                    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out rayHit))
                    {
                        if (rayHit.collider.tag == "Player")
                        {
                            playerCamera.GetComponent<GeneralManager>().totalSpice -= baseScript.ultimateCost;

                            targetCharacter = rayHit.collider.gameObject;

                            characterChosen = true;

                            Vector3 distance = CalculateAbsoluteDistance(rayHit.point);

                            if (distance.magnitude >= maximumRange)
                            {
                                characterOutOfRange = true;
                                baseScript.state = PlayerState.WALKING;
                                agent.SetDestination(targetCharacter.transform.position);
                            }
                            else
                            {
                                agent.ResetPath();
                                sadiqScript.ability3Active = true;
                            }
                        }
                    }
                }

                if (characterOutOfRange)
                {
                    Vector3 distance = CalculateAbsoluteDistance(rayHit.point);


                    if (distance.magnitude <= maximumRange)
                    {
                        sadiqScript.ability3Active = true;
                        agent.ResetPath();
                    }
                }
            } else if (baseScript.ability3Active && characterEaten && !locationChosen)
            {

                if (addLineComponentOnce)
                {
                    addLineComponentOnce = false;
                    gameObject.AddComponent<LineRenderer>();
                }

                if (gameObject.GetComponent<LineRenderer>() != null)
                    gameObject.DrawCircleScaled(maximumRange, 0.05f, transform.localScale);

                if (Input.GetKeyDown(KeyCode.Mouse0) && !onCooldown)
                {
                    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out rayHit))
                    {
                        if (rayHit.collider.tag == "Floor")
                        {
                            targetPosition = rayHit.point;

                            Vector3 distance = CalculateAbsoluteDistance(targetPosition);

                            if (distance.magnitude >= maximumRange)
                            {
                                characterOutOfRange = true;
                                baseScript.state = PlayerState.WALKING;
                                agent.SetDestination(rayHit.point);
                            }
                            else
                            {
                                agent.ResetPath();
                                LetsGoAbility sadiqLetsGo = sadiq.GetComponent<LetsGoAbility>();
                                sadiqLetsGo.pukePhase = true;
                                sadiqLetsGo.elapse_time = 0f;
                                sadiqLetsGo.targetPos = targetPosition;
                            }
                        }
                    }
                }

                if (characterOutOfRange)
                {
                    Vector3 distance = CalculateAbsoluteDistance(targetPosition);

                    if (distance.magnitude <= maximumRange)
                    {
                        agent.ResetPath();
                        LetsGoAbility sadiqLetsGo = sadiq.GetComponent<LetsGoAbility>();
                        sadiqLetsGo.pukePhase = true;
                        sadiqLetsGo.elapse_time = 0f;
                        sadiqLetsGo.targetPos = targetPosition;
                    }
                }
            }
            else
            {
                addLineComponentOnce = true;
                characterOutOfRange = false;
            }
            
            if(onCooldown)
            {
                while (elapse_time < cooldown)
                {
                    elapse_time += Time.deltaTime;
                    return;
                }

                elapse_time = 0;
                
                onCooldown = false;
            }

            
        }
    }
    void OnGUI()
    {
        if (baseScript.selectedCharacter)
            if (baseScript.ability3Active)
            {
                GUI.Box(new Rect(5, Screen.height - 30, 150, 25), "Let's Go Active");
                
                if (!characterEaten && !onCooldown) GUI.Box(new Rect(160, Screen.height - 30, 150, 25), "Select Character");
                if (characterEaten && !onCooldown) GUI.Box(new Rect(160, Screen.height - 30, 150, 25), "Select Location");

                if (onCooldown)
                    GUI.Box(new Rect(160, Screen.height - 30, 40, 25), (cooldown - elapse_time).ToString("F2"));
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
