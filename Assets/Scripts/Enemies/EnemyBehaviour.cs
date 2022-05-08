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
    DEAD,
    NONE
}
public class EnemyBehaviour : MonoBehaviour
{
    //General
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Transform player;
    [HideInInspector] public EnemyState state;

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

    //States
    private float attackRange;
    [HideInInspector] public bool playerInSightRange, playerInAttackRange;

    public EnemyType type = EnemyType.NONE;
    public LayerMask whatIsGround, whatIsPlayer;

    //Sardaukar
    private bool playerInRanged;
    private GameObject needle;
    [Header("- Only if Sardaukar -")]
    public float rangeAttackRange;
    public int ammunition;
    public GameObject needlePrefab;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        patrolIterator = 0;
        affectedByDecoy = false;
        state = EnemyState.IDLE;
        enemyD = GetComponent<EnemyDetection>();

        switch (type)
        {
            case EnemyType.HARKONNEN:
                attackRange = 2.5f;                
                break;

            case EnemyType.SARDAUKAR:
                attackRange = 2.5f;
                break;

            case EnemyType.MENTAT:
                attackRange = 2.5f;
                break;
            case EnemyType.RABBAN:
                attackRange = 3.0f;
                break;

            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check for sight and hear range
        bool detected = checkSenses();

        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!detected)
        {
            Patroling();
        }
        if (detected && !playerInAttackRange)
        {
            targetPlayerScript = player.gameObject.GetComponent<CharacterBaseBehavior>();
            Chasing();
        }

        if (detected && playerInAttackRange)
        {
            agent.ResetPath();
            targetPlayerScript = player.gameObject.GetComponent<CharacterBaseBehavior>();
            Attacking();
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
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet && !affectedByDecoy)
        {
            agent.SetDestination(walkPoint);
            state = EnemyState.WALKING;
        }


        Vector3 distanceToWalkPoint = transform.position - agent.destination;
        agent.speed = patrolingSpeed;

        //WalkPoint reached
        if (distanceToWalkPoint.magnitude < 1.5f)
        {
            walkPointSet = false;
            agent.ResetPath();
        }
    }
    private void SearchWalkPoint()
    {
        bool visited = false;


        if (patrolPoints.Count == 0) return;


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
            agent.SetDestination(player.position);
            state = EnemyState.WALKING;
            agent.speed = chasingSpeed;


            if (agent.remainingDistance <= rangeAttackRange && !agent.pathPending && ammunition > 0)
            {
                agent.ResetPath();
                RangeAttacking();
            }

        }

        if (type == EnemyType.HARKONNEN)
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
        state = EnemyState.IDLE;

        transform.LookAt(targetPlayerScript.transform);

        while (elapse_time < timeBetweenAttacks)
        {
            elapse_time += Time.deltaTime;
            return;
        }

        elapse_time = 0;

        state = EnemyState.ATTACKING;
        Vector3 offset = new Vector3(0.8f, 1.0f, 0f);
        Vector3 spawnPoint =  transform.position + (transform.rotation * offset);
        needle = Instantiate(needlePrefab, spawnPoint, transform.rotation);
        needle.transform.LookAt(targetPlayerScript.transform);

        ammunition--;
    }
}
