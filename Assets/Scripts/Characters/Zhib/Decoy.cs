using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


public class Decoy : MonoBehaviour
{
    private DecoyAbility decoyScript;
    private NavMeshAgent agent;

    public float soundRange;
    public LayerMask whatIsEnemy;

    public float firingAngle = 45.0f;
    public float gravity;

    public float effectTime;
    private float effectTimer;

    private float pulseFreq;
    private float pulseFreqTimer;

    private List<GameObject> resetedEnemies = new List<GameObject>();

    private float elapse_time;

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = GameObject.Find("Zhib");
        decoyScript = go.GetComponent<DecoyAbility>();

        soundRange = decoyScript.effectRange;

        pulseFreq = 1f;
        pulseFreqTimer = 0f;

        effectTime = 5f;
        effectTimer = 0f;

        elapse_time = 0f;

        StartCoroutine(SimulateProjectile());
    }

    IEnumerator SimulateProjectile()
    {

        // Calculate distance to target
        float target_Distance = Vector3.Distance(gameObject.transform.position, decoyScript.targetPosition);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * 1.25f * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        gameObject.transform.rotation = Quaternion.LookRotation(decoyScript.targetPosition - gameObject.transform.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration - 0.3)
        {
            gameObject.transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }

        gameObject.layer = 10;
    }

    void Update()
    {
        if (gameObject.layer == 10)
        {
            if(effectTimer < effectTime)
            {
                effectTimer += Time.deltaTime;
            } else
            {
                return;
            }

            if (pulseFreqTimer < pulseFreq)
            {
                pulseFreqTimer += Time.deltaTime;
                return;
            }
            gameObject.GetComponent<Rigidbody>().isKinematic = true;

            EmitSound();
            ResetNearEnemies();

            pulseFreqTimer = 0;
        }
    }

    void ResetNearEnemies()
    {
        Collider[] affectedEnemies = Physics.OverlapSphere(transform.position, 2f, whatIsEnemy);

        bool reseted = false;
        
        for(int j = 0; j < affectedEnemies.Length; j++)
        {
            for (int i = 0; i < resetedEnemies.Count; i++)
            {
                if (resetedEnemies[i] == affectedEnemies[j].gameObject)
                {
                    reseted = true;
                }
            }

            //if(!reseted)
            //{
                resetedEnemies.Add(affectedEnemies[j].gameObject);
                agent = affectedEnemies[j].gameObject.GetComponent<NavMeshAgent>();
                agent.ResetPath();

                EnemyBehaviour eB = affectedEnemies[j].gameObject.GetComponent<EnemyBehaviour>();
                eB.state = EnemyState.IDLE;
                eB.affectedByDecoy = false;
            //}

            reseted = false;
        }
        
    }

    void EmitSound()
    {
        Collider[] affectedEnemies = Physics.OverlapSphere(transform.position, soundRange, whatIsEnemy);

        for (int i = 0; i < affectedEnemies.Length; i++)
        {
            EnemyBehaviour eB = affectedEnemies[i].gameObject.GetComponent<EnemyBehaviour>();
            eB.state = EnemyState.WALKING;

            EnemyDetection eD = affectedEnemies[i].gameObject.GetComponent<EnemyDetection>();
            switch(eD.state)
            {
                case DecState.STILL:
                    agent = affectedEnemies[i].gameObject.GetComponent<NavMeshAgent>();
                    agent.SetDestination(transform.position);
                    eB.affectedByDecoy = true;
                    break;

                case DecState.SEEKING:
                    break;

                case DecState.FOUND:
                    break;
            }

        }
    }
}
