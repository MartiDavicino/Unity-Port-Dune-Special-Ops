using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class WeirdingWay : MonoBehaviour
{

    public CharacterBaseBehavior baseScript;

    public Camera playerCamera;
    public NavMeshAgent agent;
    public int maxKills;
    public float killProximityRange;
    public float killChainRange;
    public LayerMask whatIsEnemy;

    public Vector3 attackPointOffset;

    private RaycastHit rayHit;
    private GameObject targetedEnemy;
    private GameObject closestEnemy;
    private float closestEnemyDistance;
    private bool enemyTargeted;
    private bool firstEnemyReached;
    private int killCount;
    private Animator zhibAnimator;
    private bool addLineComponentOnce;

    private float pulseRate; //In Seconds
    private float waitTimer;


    // Start is called before the first frame update
    void Start()
    {
        zhibAnimator = GetComponent<Animator>();
        addLineComponentOnce = true;
        firstEnemyReached = false;
        enemyTargeted = false;
        pulseRate = 0.15f;
        killCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(baseScript.selectedCharacter)
        {

            if (Input.GetKeyDown(KeyCode.Alpha3))
                addLineComponentOnce = true;

            if (baseScript.ability3Active)
            {
                if(addLineComponentOnce)
                {
                    addLineComponentOnce = false;
                    gameObject.AddComponent<LineRenderer>();
                }


                gameObject.DrawCircle(killProximityRange * 6, .05f);

                agent.ResetPath();

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out rayHit))
                    {
                        if (rayHit.collider.tag == "Enemy")
                        {
                            baseScript.ability3Active = false;
                            baseScript.abilityActive = false;
                            enemyTargeted = true;
                            targetedEnemy = rayHit.collider.gameObject;
                            agent.SetDestination(rayHit.collider.gameObject.transform.position);
                            zhibAnimator.SetTrigger("isWalking");
                        }
                    }
                }

            } else {

                if (!agent.pathPending && agent.remainingDistance < killProximityRange && enemyTargeted)
                {
                    gameObject.DrawCircle(killChainRange * 6, .05f);
                    agent.ResetPath();
                    firstEnemyReached = true;
                }

            }


            if (firstEnemyReached) StartKillChain();
        } else
        {
            firstEnemyReached = false;
            enemyTargeted = false;
            addLineComponentOnce = true;
            killCount = 0;
        }
    }

    void StartKillChain()
    {
        if(waitTimer >= pulseRate)
        {
            KillChain();
            waitTimer = 0f;
        } else
        {
            waitTimer += 1 * Time.deltaTime;
        }
        
    }
    void KillChain()
    {
        Collider[] affectedEnemies = Physics.OverlapSphere(transform.position, killChainRange, whatIsEnemy);

        if (affectedEnemies.Length != 0)
        {
            if (killCount < maxKills)
            {
            zhibAnimator.SetTrigger("hasStopped");
            transform.position = targetedEnemy.transform.position + (targetedEnemy.transform.rotation * attackPointOffset);
            transform.LookAt(targetedEnemy.transform);
            Destroy(targetedEnemy);
            killCount++;

            
            
                for(int i = 0; i < affectedEnemies.Length; i++)
                {

                    if(affectedEnemies[i].gameObject != targetedEnemy)
                    {
                        Vector3 distance = CalculateAbsoluteDistance(affectedEnemies[i].gameObject);

                        if (distance.magnitude < closestEnemyDistance || closestEnemyDistance == 0)
                        {
                            closestEnemyDistance = distance.magnitude;
                            closestEnemy = affectedEnemies[i].gameObject;
                        }
                    }
                }
                closestEnemyDistance = 0;
                targetedEnemy = closestEnemy;
            
            }
            else
            {
                firstEnemyReached = false;
                enemyTargeted = false;
                addLineComponentOnce = true;
                killCount = 0;

                Destroy(gameObject.GetComponent<LineRenderer>());
            }
        }
        else
        {
            firstEnemyReached = false;
            enemyTargeted = false;
            addLineComponentOnce = true;
            killCount = 0;

            Destroy(gameObject.GetComponent<LineRenderer>());
        }
    }

    Vector3 CalculateAbsoluteDistance(GameObject enemy)
    {
        Vector3 distance = new Vector3(0f,0f,0f);

        distance.x = Mathf.Abs(transform.position.x - enemy.transform.position.x);
        //distance.y = Mathf.Abs(transform.position.y - enemy.transform.position.z);
        distance.z = Mathf.Abs(transform.position.z - enemy.transform.position.z);

        return distance;
    }

    void OnGUI()
    {
        if (baseScript.ability3Active) GUI.Box(new Rect(0, Screen.height - 25, 150, 25), "Weirding Way Active");
    }
}
