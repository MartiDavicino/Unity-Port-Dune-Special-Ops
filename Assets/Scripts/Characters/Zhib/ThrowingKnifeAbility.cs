using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


public class ThrowingKnifeAbility : MonoBehaviour
{

    public characterwalkingscript walkingScript;

    //General Variables
    public Camera playerCamera;
    private NavMeshAgent agent;
    public Transform attackPoint;
    public Vector3 attackPointOffset;
    private RaycastHit rayHit;
    private bool enemyOutOfRange;
    private Vector3 spawnPoint;
    private Animator zhibAnimator;
    private bool addLineComponentOnce;

    //Ability Stats
    public float maximumRange;
    public int ammunition;

    //Knife
    public GameObject knifePrefab;
    public LayerMask whatIsKnife;
    private GameObject[] thrownKnifes;

    // Start is called before the first frame update
    void Start()
    {
        addLineComponentOnce = true;
        enemyOutOfRange = false;
        thrownKnifes = new GameObject[ammunition];

        zhibAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(walkingScript.selectedCharacter)
        {
            if (walkingScript.ability1Active)
            {
                if (addLineComponentOnce)
                {
                    addLineComponentOnce = false;
                    gameObject.AddComponent<LineRenderer>();
                }

                gameObject.DrawCircle(maximumRange * 6, .05f);

                if (Input.GetKeyDown(KeyCode.Mouse0) && ammunition > 0)
                {
                    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out rayHit))
                    {
                        if (rayHit.collider.tag == "Enemy")
                        {
                            Vector3 distance = CalculateAbsoluteDistance(rayHit.point);

                            if (distance.magnitude >= maximumRange)
                            {
                                enemyOutOfRange = true;
                                if (zhibAnimator != null)
                                {
                                    zhibAnimator.SetTrigger("isWalking");
                                }
                                agent.SetDestination(rayHit.collider.gameObject.transform.position);
                            } else
                            {
                                spawnPoint = attackPoint.position + (attackPoint.rotation * attackPointOffset);

                                for (int i = 0; i < thrownKnifes.Length; i++)
                                {
                                    if (thrownKnifes[i] == null)
                                    {
                                        thrownKnifes[i] = Instantiate(knifePrefab, spawnPoint, attackPoint.rotation);
                                        thrownKnifes[i].transform.LookAt(rayHit.collider.gameObject.transform);
                                        break;
                                    }
                                }

                                ammunition--;
                            }
                        }
                    }
                }
            } else
            {
                addLineComponentOnce = true;

            }


            Collider[] pickableKnifes = Physics.OverlapSphere(transform.position, 3.0f, whatIsKnife);

            for(int i = 0; i < pickableKnifes.Length; i++)
            {
                Destroy(pickableKnifes[i].gameObject);
                ammunition++;
            }
        } else
        {
            addLineComponentOnce = true;
        }

    }

    private void LateUpdate()
    {
        if(walkingScript.selectedCharacter)
        {
            if (enemyOutOfRange)
            {
                if (agent.remainingDistance <= maximumRange && !agent.pathPending)
                {
                    if (zhibAnimator != null)
                    {
                        zhibAnimator.SetTrigger("hasStopped");
                    }
                    Vector3 spawnPoint = attackPoint.position + (attackPoint.rotation * attackPointOffset);

                    for (int i = 0; i < thrownKnifes.Length; i++)
                    {
                        if (thrownKnifes[i] == null)
                        {
                            thrownKnifes[i] = Instantiate(knifePrefab, spawnPoint, attackPoint.rotation);
                            thrownKnifes[i].transform.LookAt(agent.destination);
                            break;
                        }
                    }

                    agent.ResetPath();
                    ammunition--;
                    enemyOutOfRange = false;
                }
            }
        }
        
    }

    void OnGUI()
    {
        if(walkingScript.selectedCharacter)
            if (walkingScript.ability1Active) GUI.Box(new Rect(0, Screen.height - 25, 150, 25), "Throwing Knife Active");
    }

    Vector3 CalculateAbsoluteDistance(Vector3 targetPos)
    {
        Vector3 distance = new Vector3(0f, 0f, 0f);

        distance.x = Mathf.Abs(transform.position.x - targetPos.x);
        //distance.y = Mathf.Abs(transform.position.y - targetPos.z);
        distance.z = Mathf.Abs(transform.position.z - targetPos.z);

        return distance;
    }
}
