using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThrowingKnifeAbility : MonoBehaviour
{
    //General Variables
    public Camera playerCamera;
    public Transform attackPoint;
    public Vector3 attackPointOffset;
    private RaycastHit rayHit;
    private bool knifeThrown;

    //Ability Stats
    public float maximumRange;
    public int ammunition;
    public float fireRate;

    //Knife
    public GameObject knifePrefab;
    private GameObject[] thrownKnifes;
    public float knifeVelocity;
    public float soundRange;

    // Start is called before the first frame update
    void Start()
    {
        thrownKnifes = new GameObject[ammunition];
    }

    // Update is called once per frame
    void Update()
    {
       
        knifeThrown = false;

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out rayHit))
        {
            if (rayHit.collider.tag == "Enemy")
            {
                if (Input.GetKeyDown(KeyCode.Mouse0) && ammunition > 0)
                {
                    if (!knifeThrown)
                    {
                        knifeThrown = true;
                            
                        Vector3 spawnPoint = attackPoint.position + (attackPoint.rotation * attackPointOffset);

                        for (int i = 0; i < thrownKnifes.Length; i++)
                        {
                            if (thrownKnifes[i] == null)
                            {
                                thrownKnifes[i] = Instantiate(knifePrefab, spawnPoint, attackPoint.rotation);
                                thrownKnifes[i].transform.LookAt(rayHit.collider.gameObject.transform);
                                break;
                            }
                        }
 
                        ammunition--;
                    }
                }
            }
        }       
    }


    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == "Knife")
        {
            Destroy(coll.gameObject);
            ammunition++;
        }

    }

}
