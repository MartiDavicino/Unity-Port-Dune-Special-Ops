using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


public class ThrowingKnifeAbility : MonoBehaviour
{

    private CharacterBaseBehavior baseScript;

    //General Variables
    private Camera playerCamera;
    private NavMeshAgent agent;

    private Vector3 spawnPoint;
    private Vector3 attackPointOffset;

    private RaycastHit rayHit;

    private bool enemyOutOfRange;
    private bool addLineComponentOnce;

    private GameObject targetEnemy;

    //Ability Stats
    public float maximumRange;
    public int ammunition;

    //Knife
    private bool hasShot;
    public float knifeVelocity;
    public GameObject knifePrefab;
    public LayerMask whatIsKnife;
    private GameObject[] thrownKnifes;


    // Start is called before the first frame update
    void Start()
    {
        baseScript = GetComponent<CharacterBaseBehavior>();
        playerCamera = Camera.main;

        attackPointOffset = new Vector3(0.8f, 1.5f, 0);

        hasShot = false;

        addLineComponentOnce = true;
        enemyOutOfRange = false;
        thrownKnifes = new GameObject[ammunition];

        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasShot || baseScript.state == PlayerState.ABILITY1)
        {
            baseScript.state = PlayerState.IDLE;
            hasShot = false;
        }

        if (baseScript.selectedCharacter)
        {
            if (baseScript.ability1Active)
            {
                if (addLineComponentOnce)
                {
                    addLineComponentOnce = false;
                    gameObject.AddComponent<LineRenderer>();
                }

                gameObject.DrawCircleScaled(maximumRange, 0.05f, transform.localScale);

                if (Input.GetKeyDown(KeyCode.Mouse0) && ammunition > 0)
                {
                    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out rayHit))
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
                            } else
                            {
                                spawnPoint = transform.position + (transform.rotation * attackPointOffset);

                                for (int i = 0; i < thrownKnifes.Length; i++)
                                {
                                    if (thrownKnifes[i] == null)
                                    {
                                        thrownKnifes[i] = Instantiate(knifePrefab, spawnPoint, transform.rotation);
                                        thrownKnifes[i].transform.LookAt(targetEnemy.transform);
                                        break;
                                    }
                                }

                                gameObject.transform.LookAt(targetEnemy.transform);
                                baseScript.state = PlayerState.ABILITY1;
                                hasShot = true;
                                ammunition--;
                            }
                        }
                    }
                }

                if (enemyOutOfRange)
                {
                    if (agent.remainingDistance <= maximumRange && !agent.pathPending)
                    {
                        Vector3 spawnPoint = transform.position + (transform.rotation * attackPointOffset);

                        for (int i = 0; i < thrownKnifes.Length; i++)
                        {
                            if (thrownKnifes[i] == null)
                            {
                                thrownKnifes[i] = Instantiate(knifePrefab, spawnPoint, transform.rotation);
                                thrownKnifes[i].transform.LookAt(targetEnemy.transform);
                                break;
                            }
                        }

                        agent.ResetPath();
                        gameObject.transform.LookAt(targetEnemy.transform);
                        baseScript.state = PlayerState.ABILITY1;
                        hasShot = true;
                        ammunition--;
                        enemyOutOfRange = false;
                    }
                }
            } else
            {
                addLineComponentOnce = true;
                enemyOutOfRange = false;
            }

            Collider[] pickables = Physics.OverlapSphere(transform.position, 3.0f, whatIsKnife);

            for(int i = 0; i < pickables.Length; i++)
            {
                if(pickables[i].gameObject.tag == "Knife")
                {
                    Destroy(pickables[i].gameObject);
                    //ammunition++; TESTING
                }

            }
        } else
        {
            addLineComponentOnce = true;
        }

    }

    void OnGUI()
    {
        if (baseScript.selectedCharacter)
            if (baseScript.ability1Active)
            {
                GUI.Box(new Rect(5, Screen.height - 30, 150, 25), "Throwing Knife Active");
                GUI.Box(new Rect(160, Screen.height - 30, 150, 25), "Remaining Knifes:");
                GUI.Box(new Rect(315, Screen.height - 30, 30, 25), ammunition.ToString());
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
