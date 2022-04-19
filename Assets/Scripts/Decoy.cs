using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Decoy : MonoBehaviour
{
    public Rigidbody rb;

    public float soundRange;

    public LayerMask whatIsEnemy;

    public float horizontalForce;
    public float verticalForce;

    private bool applyOnce;

    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        applyOnce = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (applyOnce)
        {
            rb.AddForce(transform.TransformDirection(Vector3.forward) * horizontalForce);
            rb.AddForce(transform.TransformDirection(Vector3.up) * verticalForce);
            applyOnce = false;
        }
    }

    void OnCollisionEnter(Collision coll)
    {

        if (coll.gameObject.tag == "Floor")
        {
            EmitSound();
            transform.gameObject.layer = 10;

        }
    }

    void EmitSound()
    {
        Collider[] affectedEnemies = Physics.OverlapSphere(transform.position, soundRange, whatIsEnemy);

        for(int i = 0; i < affectedEnemies.Length; i++)
        {
            agent = affectedEnemies[i].gameObject.GetComponent<NavMeshAgent>();
            agent.SetDestination(transform.position);
        }

    }

}
