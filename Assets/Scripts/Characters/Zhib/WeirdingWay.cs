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
    private bool addLineComponentOnce;
    private bool destroyLineComponentOnce;
    private bool hasEnded;
    private bool goingToAttack;

    private float pulseRate; //In Seconds
    private float waitTimer;

    private Collider[] affectedEnemies;

    // Start is called before the first frame update
    void Start()
    {
        destroyLineComponentOnce = true;
        goingToAttack = false;
        hasEnded = false;
        addLineComponentOnce = true;
        firstEnemyReached = false;
        enemyTargeted = false;
        pulseRate = 0.15f;
        killCount = 0;
        agent.ResetPath();
    }

    // Update is called once per frame
    void Update()
    {

        if (baseScript.selectedCharacter)
        {

            if (Input.GetKeyDown(KeyCode.Alpha3) && !goingToAttack)
                addLineComponentOnce = true;

            if (baseScript.ability3Active && !hasEnded)
            {
                if(addLineComponentOnce)
                {
                    addLineComponentOnce = false;
                    gameObject.AddComponent<LineRenderer>();
                    gameObject.DrawCircle(killProximityRange * 10, .05f);
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out rayHit))
                    {
                        if (rayHit.collider.tag == "Enemy")
                        {
                            goingToAttack = true;
                            enemyTargeted = true;
                            destroyLineComponentOnce = true;
                            targetedEnemy = rayHit.collider.gameObject;
                            agent.SetDestination(rayHit.collider.gameObject.transform.position);
                            baseScript.state = PlayerState.WALKING;
                        }
                    }
                }

            } else
            {
                goingToAttack = false;

                firstEnemyReached = false;
                enemyTargeted = false;
                addLineComponentOnce = true;
                killCount = 0;
            }

            if (goingToAttack && destroyLineComponentOnce)
            {
                Destroy(gameObject.GetComponent<LineRenderer>());
                destroyLineComponentOnce = false;
            }

            if (!agent.pathPending && agent.remainingDistance < killProximityRange && enemyTargeted && goingToAttack)
            {
                gameObject.AddComponent<LineRenderer>();
                gameObject.DrawCircle(killChainRange * 10, .05f);
                agent.ResetPath();
                firstEnemyReached = true;
            }

            if (firstEnemyReached)
            {
                goingToAttack = false;

                affectedEnemies = Physics.OverlapSphere(transform.position, killChainRange, whatIsEnemy);
                StartKillChain();
            }

        } else
        {
            goingToAttack = false;

            firstEnemyReached = false;
            enemyTargeted = false;
            addLineComponentOnce = true;
            killCount = 0;
        }

   
    }

    void LateUpdate()
    {
        if (hasEnded || baseScript.state == PlayerState.ABILITY3_1)
        {
            baseScript.state = PlayerState.IDLE;
            gameObject.DrawCircle(killProximityRange * 10, .05f);
            hasEnded = false;

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
        if (killCount < maxKills)
        {
            baseScript.state = PlayerState.IDLE;
            transform.position = targetedEnemy.transform.position + (targetedEnemy.transform.rotation * attackPointOffset);
            transform.LookAt(targetedEnemy.transform);
            Destroy(targetedEnemy);
            killCount++;

            affectedEnemies = Physics.OverlapSphere(transform.position, killChainRange, whatIsEnemy);

            if (affectedEnemies.Length > 1)
            {
                for (int i = 0; i < affectedEnemies.Length; i++)
                {

                    if (affectedEnemies[i].gameObject != targetedEnemy)
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

                baseScript.state = PlayerState.ABILITY3_1;

                firstEnemyReached = false;
                enemyTargeted = false;
                killCount = 0;

            }
        }
        else
        {
            hasEnded = true;

            baseScript.state = PlayerState.ABILITY3_1;

            firstEnemyReached = false;
            enemyTargeted = false;
            killCount = 0;

        }
    }

    Vector3 CalculateAbsoluteDistance(GameObject enemy)
    {
        Vector3 distance = new Vector3(0f,0f,0f);

        distance.x = Mathf.Abs(transform.position.x - enemy.transform.position.x);
        distance.z = Mathf.Abs(transform.position.z - enemy.transform.position.z);

        return distance;
    }

    void OnGUI()
    {
        if (baseScript.ability3Active) GUI.Box(new Rect(0, Screen.height - 25, 150, 25), "Weirding Way Active");
    }
}
