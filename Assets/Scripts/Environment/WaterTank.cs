using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class WaterTank : MonoBehaviour
{

    public LayerMask whatIsEnemy;
    public LayerMask whatIsPlayer;

    private NavMeshAgent eAgent;

    public bool active;

    public GameObject[] affectedEnemies;
    private List<GameObject> resetedEnemies;
    private bool reseted;

    private bool once;

    private bool canActivate;

    // Start is called before the first frame update
    void Start()
    {
        resetedEnemies = new List<GameObject>();
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
                EnemyBehaviour eB = affectedEnemies[i].GetComponent<EnemyBehaviour>();
                eB.state = EnemyState.WALKING;
                eB.affectedByWaterTank = true;
                eB.walkPointSet = true;

                eAgent = affectedEnemies[i].GetComponent<NavMeshAgent>();
                Vector3 waterTankPos = transform.position;
                waterTankPos.y = 0;
                eB.walkPoint = waterTankPos;
                eAgent.SetDestination(waterTankPos);

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
            reseted = false;

            for (int j = 0; j < resetedEnemies.Count; j++)
            {
                if (resetedEnemies[j] == affectedEnemies[i].gameObject)
                    reseted = true;
            }


            if(!reseted)
            {
                EnemyBehaviour eB = affectedEnemies[i].gameObject.GetComponent<EnemyBehaviour>();
                eB.resetedByWaterTank = true;
                resetedEnemies.Add(affectedEnemies[i].gameObject);
            }

        }
    }
}
