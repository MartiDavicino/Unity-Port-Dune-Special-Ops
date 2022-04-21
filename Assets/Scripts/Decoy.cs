using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class Decoy : MonoBehaviour
{
    private DecoyAbility decoyScript;
    private NavMeshAgent agent;

    public float soundRange;
    public LayerMask whatIsEnemy;

    public float firingAngle = 45.0f;
    public float gravity;


    // Start is called before the first frame update
    void Start()
    {
        GameObject go = GameObject.Find("Zhib");
        decoyScript = go.GetComponent<DecoyAbility>();

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

        EmitSound();

        gameObject.layer = 10;
    }

    void EmitSound()
    {
        Collider[] affectedEnemies = Physics.OverlapSphere(transform.position, soundRange, whatIsEnemy);

        for (int i = 0; i < affectedEnemies.Length; i++)
        {
            agent = affectedEnemies[i].gameObject.GetComponent<NavMeshAgent>();
            agent.SetDestination(transform.position);
        }
    }
}
