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

    public GameObject[] affectedEnemies;

    public GameObject[] characters;

    private bool once;

    private Camera playerCamera;

    private bool going;

    // Start is called before the first frame update
    void Start()
    {
        once = false;
        going = false;
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{

        //    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit meshHit;

        //    if (Physics.Raycast(ray, out meshHit))
        //    {
        //        if (meshHit.collider.gameObject.name == "Water Tank")
        //        {
        //            going = true;
        //        }
        //    }
        //}

        //if (going && )
        for(int i = 0; i < characters.Length; i++)
        {
            if (Vector3.Distance(characters[i].transform.position, transform.position) < 3.5f)
            {  
                active = true;
            }
            //print(Vector3.Distance(characters[i].transform.position, transform.position));
        }

            if (active && !once)
        {
            for (int i = 0; i < affectedEnemies.Length; i++)
            {
                EnemyBehaviour eB = affectedEnemies[i].gameObject.GetComponent<EnemyBehaviour>();
                eB.state = EnemyState.WALKING;

                agent = affectedEnemies[i].gameObject.GetComponent<NavMeshAgent>();
                agent.SetDestination(transform.position);
            }
            ResetNearEnemies();
            once = true;
        }
       
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
