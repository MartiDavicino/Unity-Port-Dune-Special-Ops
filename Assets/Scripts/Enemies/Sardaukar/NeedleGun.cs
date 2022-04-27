using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleGun : MonoBehaviour
{
    public Rigidbody rb;

    public float velocity;

    private bool hit;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
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

    void OnCollisionEnter(Collision coll)
    {

        if (coll.gameObject.tag == "Player")
        {
            GameObject go = coll.gameObject;
            CharacterBaseBehavior baseScript = go.GetComponent<CharacterBaseBehavior>();
            baseScript.playerHealth--;
            Destroy(gameObject);
        }

        if (coll.gameObject.tag == "Floor")
        {
            Destroy(gameObject);
        }


        if (coll.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }

    }
}
