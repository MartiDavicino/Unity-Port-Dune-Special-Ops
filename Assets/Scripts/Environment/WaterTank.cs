using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class WaterTank : MonoBehaviour
{

    public float soundRange;
    public LayerMask whatIsEnemy;

    private NavMeshAgent agent;

    public bool active;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] affectedEnemies = Physics.OverlapSphere(transform.position, soundRange, whatIsEnemy);

        for (int i = 0; i < affectedEnemies.Length; i++)
        {
            EnemyBehaviour eB = affectedEnemies[i].gameObject.GetComponent<EnemyBehaviour>();
            eB.state = EnemyState.WALKING;

            agent = affectedEnemies[i].gameObject.GetComponent<NavMeshAgent>();
            agent.SetDestination(transform.position);
        }
        ResetNearEnemies();
    }

    void ResetNearEnemies()
    {
        Collider[] affectedEnemies = Physics.OverlapSphere(transform.position, 5f, whatIsEnemy);

        for (int i = 0; i < affectedEnemies.Length; i++)
        {
            agent = affectedEnemies[i].gameObject.GetComponent<NavMeshAgent>();
            agent.ResetPath();

            EnemyBehaviour eB = affectedEnemies[i].gameObject.GetComponent<EnemyBehaviour>();
            eB.state = EnemyState.IDLE;
        }
    }
}
