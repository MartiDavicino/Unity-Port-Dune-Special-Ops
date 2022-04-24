using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


public class ThrowingKnifeAbility : MonoBehaviour
{

    public CharacterBaseBehavior baseScript;

    //General Variables
    public Camera playerCamera;
    private NavMeshAgent agent;
    public Transform attackPoint;
    public Vector3 attackPointOffset;
    private RaycastHit rayHit;
    private bool enemyOutOfRange;
    private Vector3 spawnPoint;
    private bool addLineComponentOnce;

    //Ability Stats
    public float maximumRange;
    public int ammunition;

    //Knife
    private bool hasShot;
    public GameObject knifePrefab;
    public LayerMask whatIsKnife;
    private GameObject[] thrownKnifes;

    private GameObject targetEnemy;

    // Start is called before the first frame update
    void Start()
    {
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

                gameObject.DrawCircle(maximumRange * 6, .05f);

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
                                spawnPoint = attackPoint.position + (attackPoint.rotation * attackPointOffset);

                                for (int i = 0; i < thrownKnifes.Length; i++)
                                {
                                    if (thrownKnifes[i] == null)
                                    {
                                        thrownKnifes[i] = Instantiate(knifePrefab, spawnPoint, attackPoint.rotation);
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
                        Vector3 spawnPoint = attackPoint.position + (attackPoint.rotation * attackPointOffset);

                        for (int i = 0; i < thrownKnifes.Length; i++)
                        {
                            if (thrownKnifes[i] == null)
                            {
                                thrownKnifes[i] = Instantiate(knifePrefab, spawnPoint, attackPoint.rotation);
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

    void OnGUI()
    {
        if(baseScript.selectedCharacter)
            if (baseScript.ability1Active) GUI.Box(new Rect(0, Screen.height - 25, 150, 25), "Throwing Knife Active");
    }

    Vector3 CalculateAbsoluteDistance(Vector3 targetPos)
    {
        Vector3 distance = new Vector3(0f, 0f, 0f);

        distance.x = Mathf.Abs(transform.position.x - targetPos.x);
        distance.z = Mathf.Abs(transform.position.z - targetPos.z);

        return distance;
    }
}
