using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public enum EnemyType
{
    HARKONNEN,
    SARDAUKAR,
    MENTAT,
    RABBAN,
    NONE,
}

public enum EnemyState
{
    IDLE,
    WALKING,
    ATTACKING,
    RANGED,
    DEAD,
    NONE
}
public class EnemyBehaviour : MonoBehaviour
{
    //General
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Transform player;
    public EnemyState state;

    private Transform placeholder1;
    private Transform placeholder2;

    private CharacterBaseBehavior targetPlayerScript;
    private EnemyDetection enemyD;
    

    private float elapse_time = 0;

    public float chasingSpeed;
    public float patrolingSpeed;

    //Attacking
    public float timeBetweenAttacks;
    public float timeBetweenPatrolPoints;

    //Patrol
    [HideInInspector] public List<Vector3> visitedPoints;
    [HideInInspector] public bool affectedByDecoy;
    public List<Vector3> patrolPoints;

    private Vector3 walkPoint;
    private bool walkPointSet;
    private int patrolIterator;
    private Vector3 initPos;

    //States
    private float attackRange;
    [HideInInspector] public bool playerInSightRange, playerInAttackRange;

    [HideInInspector] public bool affectedByWaterTank;
    [HideInInspector] public bool resetedByWaterTank;
    public float resetEnemyTimeWaterT;
    private float resetEnemyTimer;

    public EnemyType type = EnemyType.NONE;
    public LayerMask whatIsGround, whatIsPlayer;


    //Harkonnen
    [Header("- Only if Harkonnen -")]
    public bool isGuard;
    public GameObject leader;
    public Vector3 guardOffset;
    
    //Sardaukar
    private bool playerInRanged;
    private GameObject needle;
    private float rangedAttackTimer;
    [Header("- Only if Sardaukar -")]
    public float rangeAttackRange;
    public int ammunition;
    public GameObject needlePrefab;

    //Mentat
    private List<GameObject> guardList = new List<GameObject>();
    [HideInInspector] public bool waveSpawned;
    private Vector3 initOffset;

    private Transform child;
    private Material materialHolder;
    private bool shootOnce;

    [Header("- Only if Mentat -")]
    public float summonTime;
    public float summonCooldown;
    public Vector3 spawnOffset;
    public GameObject harkonnenPrefab;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        initPos = gameObject.transform.position;
        patrolIterator = 0;
        affectedByDecoy = false;
        affectedByWaterTank = false;
        resetedByWaterTank = false;
        if (!isGuard) state = EnemyState.IDLE;
        

        switch (type)
        {
            case EnemyType.HARKONNEN:
                attackRange = 2.5f;                
                child = transform.Find("Harkonnen_low");
                break;

            case EnemyType.SARDAUKAR:
                rangedAttackTimer = 0f;
                attackRange = 2.5f;
                child = transform.Find("Sardaukar_low");
                shootOnce = true;
                break;

            case EnemyType.MENTAT:
                attackRange = 7f;
                waveSpawned = false;
                initOffset = spawnOffset;
                child = transform.Find("Mentat_low");
                materialHolder = child.gameObject.GetComponent<Renderer>().material;
                break;
            case EnemyType.RABBAN:
                attackRange = 3.0f;
                child = transform.Find(name + "_low");
                break;

            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        enemyD = GetComponent<EnemyDetection>();

        child.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;

        //Check for sight and hear range
        bool detected = checkSenses();

        if(player != null)
        {
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!detected)
            {
                Patroling();
            }

            if (detected && !playerInAttackRange)
            {
                targetPlayerScript = player.gameObject.GetComponent<CharacterBaseBehavior>();

                if (type == EnemyType.MENTAT)
                    Summoning();
                else
                    Chasing();
            }

            if (detected && playerInAttackRange)
            {
                agent.ResetPath();
                targetPlayerScript = player.gameObject.GetComponent<CharacterBaseBehavior>();
                if (type != EnemyType.MENTAT)
                    Attacking();
                else if (type == EnemyType.MENTAT)
                {
                    Fleeing();
                    Summoning();
                }
            }
        } else
        {
            if (!detected)
            {
                Patroling();
            }
        }

    }
    bool checkSenses()
    {
        if(!enemyD.noisyTargets.Any() && enemyD.visibleTargets.Any())
        {
            player = enemyD.visibleTargets[0];
            if (player == null) return false;


            for (int i = 1; i < enemyD.visibleTargets.Count; i++)
            {
                if(Vector3.Distance(agent.transform.position, enemyD.visibleTargets[i].position) < Vector3.Distance(agent.transform.position, player.transform.position))
                {
                    player = enemyD.visibleTargets[i];
                }
            }
            return true;
        }
        else if (enemyD.noisyTargets.Any() && !enemyD.visibleTargets.Any())
        {
            player = enemyD.noisyTargets[0];
            if (player == null) return false;

            for (int i = 1; i < enemyD.noisyTargets.Count; i++)
            {
                if (Vector3.Distance(agent.transform.position, enemyD.noisyTargets[i].position) < Vector3.Distance(agent.transform.position, player.transform.position))
                {
                    player = enemyD.noisyTargets[i];
                }
                
            }
            return true;
        }
        else if (enemyD.noisyTargets.Any() && enemyD.visibleTargets.Any())
        {
            placeholder1 = enemyD.noisyTargets[0];
            if (placeholder1 == null) return false;

            placeholder2 = enemyD.visibleTargets[0];
            if (placeholder2 == null) return false;

            for (int i = 1; i < enemyD.noisyTargets.Count; i++)
            {
                
                if (Vector3.Distance(agent.transform.position, enemyD.noisyTargets[i].position) < Vector3.Distance(agent.transform.position, placeholder1.transform.position))
                {
                    placeholder1 = enemyD.noisyTargets[i];
                }
                
            }
            for (int i = 1; i < enemyD.visibleTargets.Count; i++)
            {
                if (Vector3.Distance(agent.transform.position, enemyD.visibleTargets[i].position) < Vector3.Distance(agent.transform.position, placeholder2.transform.position))
                {
                    placeholder2 = enemyD.visibleTargets[i];
                }
                
            }
            if (Vector3.Distance(agent.transform.position, placeholder1.transform.position) < Vector3.Distance(agent.transform.position, placeholder2.transform.position))
            {
                player = placeholder1;
            }
            else
            {
                player = placeholder2;
            }
            return true;
        }
        return false;

    }

    private void Patroling()
    {
        
        if(type == EnemyType.MENTAT)
        {
            child.gameObject.GetComponent<Renderer>().material = materialHolder;
            summonTime = 0f;
        }

        if (isGuard)
        {
            if (!walkPointSet)
            {
                walkPoint = leader.transform.position + (leader.transform.rotation * guardOffset);
                agent.speed = patrolingSpeed;
                agent.SetDestination(walkPoint);
                state = EnemyState.WALKING;
            }

            if (agent.remainingDistance < 0.5f && !agent.pathPending && walkPointSet)
            {
                transform.rotation = leader.transform.rotation;
                state = EnemyState.IDLE;
                walkPointSet = false;
            }

        } else { 
        
            if (!walkPointSet) SearchWalkPoint();

            if (walkPointSet && !affectedByDecoy &&!affectedByWaterTank)
            {
                agent.SetDestination(walkPoint);
                state = EnemyState.WALKING;
            }

            if(resetedByWaterTank)
            {
                state = EnemyState.IDLE;

                while (resetEnemyTimer < resetEnemyTimeWaterT)
                {
                    resetEnemyTimer += Time.deltaTime;
                    return;
                }

                resetedByWaterTank = false;
                affectedByWaterTank = false;
                resetEnemyTimeWaterT = 0;
            }

            agent.speed = patrolingSpeed;

            //WalkPoint reached
            if (agent.remainingDistance < 0.5f && !agent.pathPending && walkPointSet)
            {
                if (isGuard) transform.rotation = leader.transform.rotation;
                state = EnemyState.IDLE;
                walkPointSet = false;
            }
        }
        
    }
    private void SearchWalkPoint()
    {
        bool visited = false;


        if (patrolPoints.Count == 0)
        {
            if (agent.remainingDistance > 0.2f && !agent.pathPending)
            {
                agent.SetDestination(initPos);
                state = EnemyState.WALKING;
                walkPointSet = false;
            } else
            {
                agent.ResetPath();
                state = EnemyState.IDLE;
            }
            return;
        }


        if (visitedPoints.Count == patrolPoints.Count)
            visitedPoints.Clear();
        else
            while (elapse_time < timeBetweenPatrolPoints && timeBetweenPatrolPoints > 0f)
            {
                elapse_time += Time.deltaTime;
                return;
            }

        for (int i = 0; i < visitedPoints.Count || visitedPoints == null; i++)
        {
            if(visitedPoints[i] == patrolPoints[patrolIterator])
            {
                visited = true;
            }
        }

        if (!visited)
        {
            elapse_time = 0;
            if(patrolIterator < patrolPoints.Count)
            {
                visitedPoints.Add(patrolPoints[patrolIterator]);
                walkPoint = patrolPoints[patrolIterator];
                walkPointSet = true;
                patrolIterator++;
            } else
            {
                patrolIterator = 0;
            }
        }
    }
    private void Chasing()
    {
        patrolIterator = 0;
        affectedByDecoy = false;

        if (type == EnemyType.SARDAUKAR)
        {

            if (agent.remainingDistance <= rangeAttackRange && !agent.pathPending && ammunition > 0)
            {
                agent.ResetPath();
                RangeAttacking();
            } else
            {
                agent.SetDestination(player.position);
                state = EnemyState.WALKING;
                agent.speed = chasingSpeed;
                shootOnce = true;
            }

        }

        if (type == EnemyType.HARKONNEN || type == EnemyType.RABBAN)
        {
            agent.SetDestination(player.position);
            state = EnemyState.WALKING;
            agent.speed = chasingSpeed;

        }
    }

    private void Attacking()
    {

        if (targetPlayerScript.playerHealth > 0 && state == EnemyState.WALKING)
        {
            state = EnemyState.ATTACKING;
            targetPlayerScript.playerHealth--;
            targetPlayerScript.hit = true;
        }

        state = EnemyState.IDLE;

        transform.LookAt(targetPlayerScript.transform);

        while (elapse_time < timeBetweenAttacks)
        {
            elapse_time += Time.deltaTime;
            return;
        }

        elapse_time = 0;

        if (targetPlayerScript.playerHealth > 0)
        {
            state = EnemyState.ATTACKING;
            targetPlayerScript.playerHealth--;
            targetPlayerScript.hit = true;
        }
    }

    private void RangeAttacking()
    {
        Vector3 offset;
        Vector3 spawnPoint;
        state = EnemyState.IDLE;

        transform.LookAt(targetPlayerScript.transform);

        if(shootOnce)
        {
            state = EnemyState.RANGED;
            offset = new Vector3(0.8f, 1.0f, 0f);
            spawnPoint = transform.position + (transform.rotation * offset);
            needle = Instantiate(needlePrefab, spawnPoint, transform.rotation);
            needle.transform.LookAt(targetPlayerScript.transform);
            shootOnce = false;
            ammunition--;
        }

        while (rangedAttackTimer < timeBetweenAttacks)
        {
            rangedAttackTimer += Time.deltaTime;
            return;
        }

        shootOnce = true;

        rangedAttackTimer = 0;

        state = EnemyState.RANGED;
        offset = new Vector3(0.8f, 1.0f, 0f);
        spawnPoint =  transform.position + (transform.rotation * offset);
        needle = Instantiate(needlePrefab, spawnPoint, transform.rotation);
        needle.transform.LookAt(targetPlayerScript.transform);

        ammunition--;
    }

    private void Summoning()
    {
        
        while(guardList.Count < 4)
        {
            if (!waveSpawned)
            {

                while (elapse_time < summonTime)
                {
                    elapse_time += Time.deltaTime;
                    child.gameObject.GetComponent<Renderer>().material = Resources.Load(name + "summon", typeof(Material)) as Material;
                    return;
                }

                child.gameObject.GetComponent<Renderer>().material = materialHolder;

                elapse_time = 0;

                for (int i = 0; i < 2; i++)
                {
                    if (guardList.Count == 1) spawnOffset.x = -initOffset.x;
                    if (guardList.Count == 2) spawnOffset.z = -initOffset.z;
                    if (guardList.Count == 3)
                    {
                        spawnOffset.x = -initOffset.x;
                        spawnOffset.z = -initOffset.z;
                    }
            
                    Vector3 spawnPoint = transform.position + (transform.rotation * spawnOffset);
                    GameObject summonedEnemy = Instantiate(harkonnenPrefab, spawnPoint, transform.rotation);
                    guardList.Add(summonedEnemy);

                    EnemyDetection summonedDetection = summonedEnemy.GetComponent<EnemyDetection>();
                    summonedDetection.state = DecState.SEEKING;
                    summonedDetection.timer = summonedDetection.secondsToDetect - 0.2f;

                    EnemyBehaviour summonedBehaviour = summonedEnemy.GetComponent<EnemyBehaviour>();
                    summonedBehaviour.walkPointSet = true;
                    summonedBehaviour.state = EnemyState.WALKING;
                    summonedBehaviour.isGuard = true;
                    summonedBehaviour.leader = gameObject;
                    summonedBehaviour.guardOffset = spawnOffset;

                    NavMeshAgent summonedAgent = summonedEnemy.GetComponent<NavMeshAgent>();
                    summonedAgent.SetDestination(player.position);

                    spawnOffset = initOffset;
                }
            
                waveSpawned = true;
            } else
            {
                while (elapse_time < summonCooldown)
                {
                    elapse_time += Time.deltaTime;
                    return;
                }

                elapse_time = 0;

                waveSpawned = false;
            }
        }

    }

    private void Fleeing()
    {
        state = EnemyState.WALKING;

        // store the starting transform
        Transform startTransform = transform;

        //temporarily point the object to look away from the player
        transform.rotation = Quaternion.LookRotation(transform.position - player.position);

        //Then we'll get the position on that rotation that's multiplyBy down the path (you could set a Random.range
        // for this if you want variable results) and store it in a new Vector3 called runTo
        Vector3 runTo = transform.position + transform.forward * agent.speed;
        //Debug.Log("runTo = " + runTo);

        //So now we've got a Vector3 to run to and we can transfer that to a location on the NavMesh with samplePosition.

        NavMeshHit hit;    // stores the output in a variable called hit

        // 5 is the distance to check, assumes you use default for the NavMesh Layer name
        NavMesh.SamplePosition(runTo, out hit, 5, 1 << NavMesh.GetNavMeshLayerFromName("Default"));
        //Debug.Log("hit = " + hit + " hit.position = " + hit.position);

        // just used for testing - safe to ignore
        //nextTurnTime = Time.time + 5;

        // reset the transform back to our start transform
        transform.position = startTransform.position;
        transform.rotation = startTransform.rotation;

        // And get it to head towards the found NavMesh position
        agent.SetDestination(hit.position);
        
    }
}
