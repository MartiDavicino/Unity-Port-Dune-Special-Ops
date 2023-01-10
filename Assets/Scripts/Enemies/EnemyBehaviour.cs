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

    [Header("- IMPORTANT - True If Playtesting -")]
    public bool safe = false;

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Transform player;
    //General
    [Header("- General Stats -")]
    public float chasingSpeed;
    public float patrolingSpeed;
    public EnemyState state;
    
    [HideInInspector] public bool affectedByWaterTank;
    [HideInInspector] public bool resetedByWaterTank;
    [HideInInspector] public float resetEnemyTimeWaterT;
    private float resetEnemyTimer;

    //Attacking
    private bool instaHit;
    public float timeBetweenAttacks;

    private Transform placeholder1;
    private Transform placeholder2;
    private CharacterBaseBehavior targetPlayerScript;
    private EnemyDetection enemyD;
    private float elapse_time = 0;

    //Patrol
    [HideInInspector] public List<Vector3> visitedPoints;
    [HideInInspector] public bool affectedByDecoy;
    public float timeBetweenPatrolPoints;
    public List<Vector3> patrolPoints;

    [HideInInspector] public Vector3 walkPoint;
    [HideInInspector] public bool walkPointSet;
    private int patrolIterator;
    private Vector3 initPos;
    private Quaternion initRot;

    //States
    private float attackRange;
    [HideInInspector] public bool playerInSightRange, playerInAttackRange;


    public EnemyType type = EnemyType.NONE;
    public LayerMask whatIsGround, whatIsPlayer;


    //Harkonnen
    [Header("- Only if Harkonnen -")]
    public bool isGuard;
    public Vector3 guardOffset;
    public GameObject leader;
    [Range(0.0f, 1.0f)]
    public float harkonnenDropChance;
    public int harkonnenMaxDrop;
    public int harkonnenMinDrop;

    //Sardaukar
    private bool playerInRanged;
    private GameObject needle;
    private float rangedAttackTimer;
    [Header("- Only if Sardaukar -")]
    public float rangedAttackRange;
    public int ammunition;
    public GameObject needlePrefab;
    [Range(0.0f, 1.0f)]
    public float sardaukarDropChance;
    public int sardaukarMaxDrop;
    public int sardaukarMinDrop;

    //Mentat
    private List<GameObject> guardList = new List<GameObject>();
    [HideInInspector] public bool waveSpawned;
    private Vector3 initOffset;

    private Transform child;
    private Material materialHolder;
    private bool shootOnce;

    [Header("- Only if Mentat -")]
    public float summonTime;
    private float summonTimer;
    public float summonCooldown;
    public Vector3 spawnOffset;
    public GameObject harkonnenPrefab;
    [Range(0.0f, 1.0f)]
    public float mentatDropChance;
    public int mentatMaxDrop;
    public int mentatMinDrop;

    ///////////////////////////////////////////////////////////////////////
    //Esto es una guarrada pq sino no me van los putos enemigos en la build.
    //Se puede desactivar poniendo safe a true y
    //comentando la llamada a KillImpostors()
    private float spawnTimer = 0f;
    private bool initTP = false;
    private Vector3 warpInitPos;
    private bool warpOnce = true;
    ///////////////////////////////////////////////////////////////////////


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyD = GetComponent<EnemyDetection>();


        initPos = gameObject.transform.position;
        initRot = gameObject.transform.rotation;
        agent.Warp(initPos);
        patrolIterator = 0;
        instaHit = true;
        affectedByDecoy = false;
        affectedByWaterTank = false;
        resetedByWaterTank = false;
        summonTimer = 0f;
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
        child.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;

        //Check for sight and hear range
        bool detected = checkSenses();

        if(player != null)
        {
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!detected && !enemyD.isAggro)
            {
                Patroling();
            }

            if ((detected || enemyD.isAggro) && !playerInAttackRange)
            {

                visitedPoints.Clear();

                affectedByDecoy = false;
                affectedByWaterTank = false;

                targetPlayerScript = player.gameObject.GetComponent<CharacterBaseBehavior>();

                if (type == EnemyType.MENTAT)
                    Summoning();
                else
                    Chasing();
            }

            if ((detected || enemyD.isAggro) && playerInAttackRange)
            {
                visitedPoints.Clear();

                affectedByDecoy = false;
                affectedByWaterTank = false;

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

        if(!isGuard && !safe) KillImpostors();

    }

    private void KillImpostors()
    {
        if(!initTP)
        {
            if (type == EnemyType.MENTAT) return;

            while (spawnTimer < enemyD.secondsPerBar)
            {
                spawnTimer += Time.deltaTime;

                if (enemyD.state != DecState.FOUND)
                {
                    Vector3 offset = new Vector3(3f, 0, 0);
                    CameraMovement cM = Camera.main.GetComponent<CameraMovement>();
                    agent.Warp(cM.focusedPlayer.transform.position + offset);
                    warpInitPos = gameObject.transform.position;
                    transform.LookAt(cM.focusedPlayer.transform.position);
                }

                return;
            }

            if (!safe)
                Destroy(gameObject);
            else if(warpOnce)
            {
                initTP = true;
                warpOnce = false;
            }
        }
    }
    private void LateUpdate()
    {
        if(initTP)
        {
            Animator animator = gameObject.GetComponent<Animator>();
            animator.SetTrigger("hasStopped");
            enemyD.timer = 0f;
            enemyD.state = DecState.STILL;
            state = EnemyState.IDLE;
            agent.ResetPath();
            agent.Warp(initPos);
            initTP = false;
            gameObject.transform.rotation = initRot;
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
        instaHit = true;

        if (type == EnemyType.MENTAT)
        {
            child.gameObject.GetComponent<Renderer>().material = materialHolder;
            if(summonTimer > 0) summonTimer -= summonTimer * Time.deltaTime;
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
            
            if(resetedByWaterTank)
            {
                state = EnemyState.IDLE;

                while (resetEnemyTimer < resetEnemyTimeWaterT)
                {
                    resetEnemyTimer += Time.deltaTime;
                    return;
                }

                walkPointSet = true;
                walkPoint = initPos;
                agent.SetDestination(initPos);

                resetedByWaterTank = false;
                affectedByWaterTank = false;
                resetEnemyTimeWaterT = 0;
            }

            //WalkPoint reached
            if ((transform.position - walkPoint).magnitude < 0.5 && walkPointSet)
            {
                if (isGuard) transform.rotation = leader.transform.rotation;
                state = EnemyState.IDLE;
                walkPointSet = false;
            }
        
            if (!walkPointSet) SearchWalkPoint();

            if (walkPointSet && !affectedByDecoy && !affectedByWaterTank)
            {
                agent.SetDestination(walkPoint);
                state = EnemyState.WALKING;
            }

            agent.speed = patrolingSpeed;

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
                gameObject.transform.rotation = initRot;
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

        if (type == EnemyType.SARDAUKAR)
        {

            if (agent.remainingDistance <= rangedAttackRange && ammunition > 0 && safe && !agent.pathPending && !player.gameObject.GetComponent<CharacterBaseBehavior>().startInvincible)
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

        safe = true;

        if (instaHit && targetPlayerScript.playerHealth > 0 && state == EnemyState.WALKING)
        {
            state = EnemyState.ATTACKING;
            targetPlayerScript.playerHealth--;
            targetPlayerScript.hit = true;
            instaHit = false;
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
        gameObject.transform.LookAt(player.position);

        while(guardList.Count < 4)
        {
            if (!waveSpawned)
            {

                while (summonTimer < summonTime)
                {
                    summonTimer += Time.deltaTime;
                    child.gameObject.GetComponent<Renderer>().material = Resources.Load(name + "summon", typeof(Material)) as Material;
                    return;
                }

                child.gameObject.GetComponent<Renderer>().material = materialHolder;

                summonTimer = 0;

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
                    summonedDetection.timer = summonedDetection.secondsPerBar - 0.2f;

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
                while (summonTimer < summonCooldown)
                {
                    summonTimer += Time.deltaTime;
                    return;
                }

                summonTimer = 0;

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
        NavMesh.SamplePosition(runTo, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
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
