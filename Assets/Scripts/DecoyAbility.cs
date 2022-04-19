using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DecoyAbility : MonoBehaviour
{

    public characterwalkingscript walkingScript;

    //General Variables
    public Camera playerCamera;
    public Transform attackPoint;
    public Vector3 attackPointOffset;
    private GameObject decoy;
    private bool active;

    //Ability Stats
    public float maximumRange;
    public int ammunition;
    public float fireRate;

    //Decoy
    public GameObject decoyPrefab;
    public float decoyVelocity;
    public float effectRange;

    // Start is called before the first frame update
    void Start()
    {
        active = false;
    }

    // Update is called once per frame
    void Update()
    {

        if(walkingScript.ability2Active)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && ammunition > 0)
            {
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit meshHit;

                if (Physics.Raycast(ray, out meshHit))
                {
                    if (meshHit.collider.tag == "Floor")
                    {
                        transform.LookAt(meshHit.point);

                        Vector3 spawnPoint = attackPoint.position + (attackPoint.rotation * attackPointOffset);

                        decoy = Instantiate(decoyPrefab, spawnPoint, attackPoint.rotation);

                        ammunition--;
                    }
                }
            }
        }
    }
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == "Decoy")
        {
            Destroy(decoy);
            ammunition++;
        }
    }
    void OnGUI()
    {
        if (walkingScript.ability2Active) GUI.Box(new Rect(0, Screen.height - 25, 150, 25), "Decoy Active");
    }

}
