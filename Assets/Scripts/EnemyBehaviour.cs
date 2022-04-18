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

public class EnemyBehaviour : MonoBehaviour
{
    enum state
    {
        SEEK, 
        PATROL,

    }

    public EnemyType type = EnemyType.NONE;

    state currentState = state.PATROL;

    public NavMeshAgent agent;

    public Transform player;
    private Transform placeholder1;
    private Transform placeholder2;


    public LayerMask whatIsGround, whatIsPlayer;

    public FieldOfHearing foh;
    public FieldOfView fov;

    [SerializeField] public Material enemyMaterial;

    //Patrol
    private Vector3 walkPoint;
    bool walkPointSet;
    private float walkPointRange;

    //Attacking
    private float timeBetweenAttacks;
    bool alreadtAttacked;

    //States
    private float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    bool checkSenses()
    {
        if(!foh.visibleTargets.Any() && fov.visibleTargets.Any())
        {
            player = fov.visibleTargets[0];
            for (int i = 1; i < fov.visibleTargets.Count; i++)
            {
                if(Vector3.Distance(agent.transform.position,fov.visibleTargets[i].position) < Vector3.Distance(agent.transform.position, player.transform.position))
                {
                    player = fov.visibleTargets[i];
                }
            }
            return true;
        }
        else if (foh.visibleTargets.Any() && !fov.visibleTargets.Any())
        {
            player = foh.visibleTargets[0];

            for (int i = 1; i < foh.visibleTargets.Count; i++)
            {
                if (Vector3.Distance(agent.transform.position, foh.visibleTargets[i].position) < Vector3.Distance(agent.transform.position, player.transform.position))
                {
                    player = foh.visibleTargets[i];
                }
                
            }
            return true;
        }
        else if (foh.visibleTargets.Any() && fov.visibleTargets.Any())
        {
            placeholder1 = foh.visibleTargets[0];
            placeholder2 = fov.visibleTargets[0];

            for (int i = 1; i < foh.visibleTargets.Count; i++)
            {
                
                if (Vector3.Distance(agent.transform.position, foh.visibleTargets[i].position) < Vector3.Distance(agent.transform.position, placeholder1.transform.position))
                {
                    placeholder1 = foh.visibleTargets[i];
                }
                
            }
            for (int i = 1; i < fov.visibleTargets.Count; i++)
            {
                if (Vector3.Distance(agent.transform.position, fov.visibleTargets[i].position) < Vector3.Distance(agent.transform.position, placeholder2.transform.position))
                {
                    placeholder2 = fov.visibleTargets[i];
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
        //float randomZ = Random.Range(-walkPointRange, walkPointRange);
        //float randomX = Random.Range(-walkPointRange, walkPointRange);

        //walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        //if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        //    walkPointSet = true;
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

    // Start is called before the first frame update
    void Start()
    {

        switch (type)
        {
            case EnemyType.HARKONNEN:
                walkPointRange = 10.0f;
                sightRange = 10.0f;
                attackRange = 2.0f;                
                break;

            case EnemyType.SARDAUKAR:
                walkPointRange = 10.0f;
                sightRange = 15.0f;
                attackRange = 2.0f;
                break;

            case EnemyType.MENTAT:
                walkPointRange = 10.0f;
                sightRange = 20.0f;
                attackRange = 2.0f;
                break;

            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check for sight and attack range
        checkSenses();
        //playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!checkSenses())
        {
            enemyMaterial.color = Color.blue;
            Patroling();
        }
        if (checkSenses() && !playerInAttackRange)
        {
            enemyMaterial.color = Color.yellow;
            Chasing();
        }

        if (checkSenses() && playerInAttackRange)
        {
            enemyMaterial.color = Color.red;
            Attacking();
        }
    }
}
