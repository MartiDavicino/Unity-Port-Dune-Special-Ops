using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class OmozraÑamÑamAbility : MonoBehaviour
{
    private GameObject sadiq;
    private SadiqBehaviour sadiqScript;

    [HideInInspector] public CharacterBaseBehavior baseScript;

    private NavMeshAgent agent;
    private Camera playerCamera;
    private RaycastHit rayHit;

    [HideInInspector] public bool addLineComponentOnce;
    [HideInInspector] public GameObject targetEnemy;
    private bool enemyOutOfRange;

    public float maximumRange;
    
    public float cooldown;
    [HideInInspector] public bool onCooldown;
    private float elapse_time;

    [Header("- Chances To Devour -")]

    [Header("- Harkonnen -")]
    [Range(0.0f, 1.0f)]
    public float harkonnenUnaware;
    [Range(0.0f, 1.0f)]
    public float harkonnenAware;
    [Range(0.0f, 1.0f)]
    public float harkonnenDetected;

    [Header("- Sardaukar -")]
    [Range(0.0f, 1.0f)]
    public float sardaukarUnaware;
    [Range(0.0f, 1.0f)]
    public float sardaukarAware;
    [Range(0.0f, 1.0f)]
    public float sardaukarDetected;

    [Header("- Mentat -")]
    [Range(0.0f, 1.0f)]
    public float mentatUnaware;
    [Range(0.0f, 1.0f)]
    public float mentatAware;
    [Range(0.0f, 1.0f)]
    public float mentatDetected;

    // Start is called before the first frame update
    void Start()
    {
        sadiq = GameObject.Find("Sadiq");
        sadiqScript = sadiq.GetComponent<SadiqBehaviour>();

        baseScript = gameObject.GetComponent<CharacterBaseBehavior>();
        playerCamera = Camera.main;

        addLineComponentOnce = true;

        enemyOutOfRange = false;

        agent = GetComponent<NavMeshAgent>();

        onCooldown = false;
        elapse_time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(baseScript.selectedCharacter)
        {
            
            if (baseScript.ability2Active)
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

                    if (Physics.Raycast(ray, out rayHit, 1000, LayerMask.GetMask("Enemy")))
                    {
                        if (rayHit.collider.tag == "Enemy")
                        {
                            targetEnemy = rayHit.collider.gameObject;

                            Vector3 distance = CalculateAbsoluteDistance(rayHit.point);

                            if (distance.magnitude >= maximumRange)
                            {
                                enemyOutOfRange = true;
                                baseScript.state = PlayerState.WALKING;
                                agent.SetDestination(targetEnemy.transform.position);
                            }
                            else
                            {
                                agent.ResetPath();
                                sadiqScript.ability2Active = true;
                            }
                        }
                    }
                }

                if (enemyOutOfRange)
                {
                    if (agent.remainingDistance <= maximumRange && !agent.pathPending)
                    {
                        agent.ResetPath();
                        sadiqScript.ability2Active = true;
                    }
                }
            } else
            {
                addLineComponentOnce = true;
                enemyOutOfRange = false;
            }
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
    void OnGUI()
    {
        if (baseScript.selectedCharacter)
            if (baseScript.ability2Active)
            {
                GUI.Box(new Rect(5, Screen.height - 30, 150, 25), "Ñam, Ñam Active");
            
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
