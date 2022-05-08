using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnife : MonoBehaviour
{
    private Rigidbody rb;

    private float velocity;

    public float soundRange;
    public LayerMask whatIsEnemy;

    private bool hit;

    private GameObject zhib;
    private ThrowingKnifeAbility baseScript;

    // Start is called before the first frame update
    void Start()
    {
        zhib = GameObject.Find("Zhib");
        baseScript = zhib.GetComponent<ThrowingKnifeAbility>();

        soundRange = baseScript.effectRange;

        velocity = baseScript.knifeVelocity;
        hit = false;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (!hit)
        {
            transform.position += transform.rotation * Vector3.forward * velocity * Time.deltaTime;

            RaycastHit rayHit;

            if ((Physics.Raycast(transform.position, -Vector3.up, out rayHit, 10f)))
            {

                float desiredDistance = 1.5f;

                if (rayHit.distance > desiredDistance)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + desiredDistance - rayHit.distance, transform.position.z);
                } else if(rayHit.distance < desiredDistance)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + desiredDistance - rayHit.distance, transform.position.z);
                }

            }
        }


    }

    void OnCollisionEnter(Collision coll)
    {

        if (coll.gameObject.tag == "Enemy")
        {
            if(gameObject.layer != 10) Destroy(coll.gameObject);

            hit = true;

            rb.useGravity = true;

            RaycastHit rayHit;

            // If the ray casted from this object (in your case, the tree) to below it hits something...
            if ((Physics.Raycast(transform.position, -Vector3.up, out rayHit, 10f)))
            {

                // and if the distance between object and hit is larger than 0.3 (I judge it nearly unnoticeable otherwise)
                if (rayHit.distance > 0.3f)
                {
                    // Then bring object down by distance value.
                    transform.position = new Vector3(transform.position.x, transform.position.y - rayHit.distance, transform.position.z);
                }

            }
        }

        if (coll.gameObject.tag == "Floor")
        {
            transform.gameObject.layer = 10;
        }


        if (coll.gameObject.tag == "Wall")
        {
            hit = true;
            rb.useGravity = true;
            EmitSound();
        }

        
    }
    void EmitSound()
    {
        Collider[] affectedEnemies = Physics.OverlapSphere(transform.position, soundRange, whatIsEnemy);

        for (int i = 0; i < affectedEnemies.Length; i++)
        {
            affectedEnemies[i].GetComponent<EnemyDetection>().state = DecState.SEEKING;
            affectedEnemies[i].GetComponent<EnemyDetection>().timer = affectedEnemies[i].GetComponent<EnemyDetection>().secondsToDetect;

        }
    }
}
