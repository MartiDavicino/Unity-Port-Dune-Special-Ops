using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class WeirdingWay : MonoBehaviour
{

    private CharacterBaseBehavior baseScript;
    private Camera playerCamera;
    private NavMeshAgent agent;

    public int maxKills;
    public float killProximityRange;
    public float killChainRange;
    [Range(0.0f, 1.0f)]
    public float chanceToKillSardaukar;
    public float soundRange;
    public LayerMask whatIsEnemy;
    private Vector3 attackPointOffset;

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

    private int initHealth;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = GetComponent<CharacterBaseBehavior>();
        agent = GetComponent<NavMeshAgent>();
        playerCamera = Camera.main;

        destroyLineComponentOnce = true;
        goingToAttack = false;
        hasEnded = false;
        addLineComponentOnce = true;
        firstEnemyReached = false;
        enemyTargeted = false;
        pulseRate = 0.15f;
        killCount = 0;
        agent.ResetPath();

        attackPointOffset = new Vector3(0.8f, 2.3f, 0f);
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

                    gameObject.DrawCircleScaled(killProximityRange, 0.05f, transform.localScale);
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out rayHit))
                    {
                        if (rayHit.collider.tag == "Enemy")
                        {
                            initHealth = baseScript.playerHealth;
                            goingToAttack = true;
                            enemyTargeted = true;
                            destroyLineComponentOnce = true;
                            targetedEnemy = rayHit.collider.gameObject;
                            agent.SetDestination(rayHit.collider.gameObject.transform.position);
                            if(baseScript.state == PlayerState.CROUCH && baseScript.state == PlayerState.RUNNING)
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

            if (goingToAttack) baseScript.playerHealth = initHealth;

            if (!agent.pathPending && agent.remainingDistance < killProximityRange && enemyTargeted && goingToAttack)
            {
                gameObject.AddComponent<LineRenderer>();
                gameObject.DrawCircleScaled(killProximityRange, 0.05f, transform.localScale);

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
            gameObject.DrawCircleScaled(killProximityRange, 0.05f, transform.localScale);
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
            baseScript.playerHealth = initHealth;
            baseScript.state = PlayerState.IDLE;
            transform.position = targetedEnemy.transform.position + (targetedEnemy.transform.rotation * attackPointOffset);
            transform.LookAt(targetedEnemy.transform);
            
            EnemyBehaviour eB = targetedEnemy.GetComponent<EnemyBehaviour>();
            switch (eB.type)
            {
                case EnemyType.HARKONNEN:
                    Destroy(targetedEnemy);
                    break;

                case EnemyType.SARDAUKAR:
                    if (Random.value < chanceToKillSardaukar)
                    {
                        Destroy(targetedEnemy);
                    }
                    else
                    {
                        EmitSound();
                    }
                    break;

                case EnemyType.MENTAT:
                    Destroy(targetedEnemy);
                    break;
            }
            

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
                baseScript.playerHealth = initHealth;

            }
        }
        else
        {
            hasEnded = true;

            baseScript.state = PlayerState.ABILITY3_1;

            firstEnemyReached = false;
            enemyTargeted = false;
            killCount = 0;
            baseScript.playerHealth = initHealth;

        }
    }

    Vector3 CalculateAbsoluteDistance(GameObject enemy)
    {
        Vector3 distance = new Vector3(0f,0f,0f);

        distance.x = Mathf.Abs(transform.position.x - enemy.transform.position.x);
        distance.z = Mathf.Abs(transform.position.z - enemy.transform.position.z);

        return distance;
    }

    void EmitSound()
    {
        Collider[] affectedEnemies = Physics.OverlapSphere(transform.position, soundRange, whatIsEnemy);

        for (int i = 0; i < affectedEnemies.Length; i++)
        {
            affectedEnemies[i].GetComponent<EnemyDetection>().state = DecState.SEEKING;
            affectedEnemies[i].GetComponent<EnemyDetection>().timer = affectedEnemies[i].GetComponent<EnemyDetection>().secondsToDetect;

        }
    }
    void OnGUI()
    {
        if (baseScript.ability3Active) GUI.Box(new Rect(5, Screen.height - 30, 150, 25), "Weirding Way Active");
    }
}
