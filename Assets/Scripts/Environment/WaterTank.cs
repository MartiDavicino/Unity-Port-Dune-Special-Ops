using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class WaterTank : MonoBehaviour
{

    public LayerMask whatIsEnemy;
    public LayerMask whatIsPlayer;

    private NavMeshAgent agent;

    public bool active;

    public GameObject[] affectedEnemies;

    private bool once;

    private bool canActivate;

    // Start is called before the first frame update
    void Start()
    {
        once = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!active)
        {
            Collider[] affectedPlayers = Physics.OverlapSphere(transform.position, 15f, whatIsPlayer);

            if (affectedPlayers.Length > 0)
                canActivate = true;
            else
                canActivate = false;

            if(canActivate)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    active = true;
                    canActivate = false;
                }
            }
        }

        if (active && once)
        {
            for (int i = 0; i < affectedEnemies.Length; i++)
            {
                affectedEnemies[i].GetComponent<EnemyBehaviour>().state = EnemyState.WALKING;
                affectedEnemies[i].GetComponent<EnemyBehaviour>().affectedByWaterTank = true;
                affectedEnemies[i].GetComponent<NavMeshAgent>().SetDestination(transform.position);
                Destroy(affectedEnemies[i]);
            }
            once = false;
        }

        ResetNearEnemies();
       
    }

    private void OnGUI()
    {
        if (canActivate) GUI.Box(new Rect(Screen.width - 155, Screen.height - 45, 150, 40), "Press 'f' to\nactivate Water Tank");
    }
    void ResetNearEnemies()
    {
        Collider[] affectedEnemies = Physics.OverlapSphere(transform.position, 5f, whatIsEnemy);

        for (int i = 0; i < affectedEnemies.Length; i++)
        {
            EnemyBehaviour eB = affectedEnemies[i].gameObject.GetComponent<EnemyBehaviour>();
            eB.resetedByWaterTank = true;

            agent = affectedEnemies[i].gameObject.GetComponent<NavMeshAgent>();
            agent.ResetPath();
        }
    }
}
