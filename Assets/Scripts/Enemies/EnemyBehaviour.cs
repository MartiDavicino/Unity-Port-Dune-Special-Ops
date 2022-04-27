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
   

    public EnemyType type = EnemyType.NONE;

    public NavMeshAgent agent;

    public Transform player;
    private Transform placeholder1;
    private Transform placeholder2;

    private CharacterBaseBehavior targetPlayer;

    private float elapse_time = 0;

    public EnemyState state;

    public LayerMask whatIsGround, whatIsPlayer;

    public EnemyDetection enemyD;


    //Patrol
    public List<Vector3> patrolPoints;
    public List<Vector3> visitedPoints;
    private Vector3 walkPoint;
    bool walkPointSet;
    private float walkPointRange;
    private int patrolIterator;
    [HideInInspector] public bool affectedByDecoy;

    //Attacking
    public float timeBetweenAttacks;

    //States
    private float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    //Sardaukar
    private float rangeAttackRange;
    private int ammunition;
    private bool playerInRanged;
    private GameObject needle;
    public GameObject needlePrefab;

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

        for (int i = 0; i < visitedPoints.Count || visitedPoints == null; i++)
        {
            if(visitedPoints[i] == patrolPoints[patrolIterator])
            {
                visited = true;
            }
        }

        if (!visited)
        {
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
        }
    }

    private void Attacking()
    {
        state = EnemyState.IDLE;

        transform.LookAt(targetPlayer.transform);

        while (elapse_time < timeBetweenAttacks)
        {
            elapse_time += Time.deltaTime;
            return;
        }

        elapse_time = 0;
        if (targetPlayer.playerHealth > 0)
        {
            state = EnemyState.ATTACKING;
            targetPlayer.playerHealth--;
        }
    }

    private void RangeAttacking()
    {
        state = EnemyState.IDLE;

        transform.LookAt(targetPlayer.transform);

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
        needle.transform.LookAt(targetPlayer.transform);

        ammunition--;
    }

    // Start is called before the first frame update
    void Start()
    {
        patrolIterator = 0;
        affectedByDecoy = false;

        switch (type)
        {
            case EnemyType.HARKONNEN:
                walkPointRange = 10.0f;
                sightRange = 10.0f;
                attackRange = 3.0f;                
                break;

            case EnemyType.SARDAUKAR:
                walkPointRange = 10.0f;
                sightRange = 15.0f;
                attackRange = 1.0f;
                rangeAttackRange = 7.0f;
                ammunition = 2;
                break;

            case EnemyType.MENTAT:
                walkPointRange = 10.0f;
                sightRange = 20.0f;
                attackRange = 1.0f;
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

        //playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!detected)
        {
            Patroling();
        }
        if (detected && !playerInAttackRange)
        {
            targetPlayer = player.gameObject.GetComponent<CharacterBaseBehavior>();
            Chasing();
        }

        if (detected && playerInAttackRange)
        {
            agent.ResetPath();
            targetPlayer = player.gameObject.GetComponent<CharacterBaseBehavior>();
            Attacking();
        }
    }
}
