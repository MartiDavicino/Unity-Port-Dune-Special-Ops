using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnife : MonoBehaviour
{
    public Rigidbody rb;

    public float velocity;

    public bool hit;

    private float pickUpRadius;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!hit) transform.position += transform.rotation * Vector3.forward * velocity * Time.deltaTime;
    }

    void OnCollisionEnter(Collision coll)
    {

        if (coll.gameObject.tag == "Enemy")
        {
            hit = true;

            rb.useGravity = true;

            Destroy(coll.gameObject);

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

        if (coll.gameObject.tag == "Wall")
        {
            hit = true;
            rb.useGravity = true;
        }

        
    }
}
