using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    HARKONNEN,
    SARDAUKAR,
    MENTAT,
    NONE,
}

public class EnemyBehaviour : MonoBehaviour
{
    enum state
    {
        SEEK, 
        PATROL
    }

    state currentState = state.PATROL;

    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    //Patrol
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Chasing
    public Vector3 newScale = new Vector3(1.0f,1.2f,1.0f);

    //Attacking
    public float timeBetweenAttacks;
    bool alreadtAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //WalkPoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        //This are the waypoints thing
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }
    private void Chasing()
    {
        //transform.localScale += newScale;
        agent.SetDestination(player.position);
    }

    private void Attacking()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadtAttacked)
        {
            //Attack Code Here

            //

            alreadtAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadtAttacked = false;
    }

    //int speed = 20;
    //public GameObject player;
    //Transform lastPlayerPos;
    //public int minRetargetingDistance = 3;
    //public int minSeekDistance = 100;

    //List<Vector3> oldWaypoint = new List<Vector3>();
    //List<Vector3> waypoints = new List<Vector3>();
    //List<Vector3> finalPath = new List<Vector3>();
    //int currentPathIndex = 1;


    //public EnemyType type = EnemyType.NONE;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Check for sight and attack range

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) Chasing();
        if (playerInSightRange && playerInAttackRange) Attacking();
    }
}
