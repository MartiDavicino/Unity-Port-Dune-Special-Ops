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
    private bool addLineComponentOnce;

    //Ability Stats
    public float maximumRange;
    public int ammunition;
    public float fireRate;

    //Decoy
    public LayerMask whatIsDecoy;
    public GameObject decoyPrefab;
    public float decoyVelocity;
    public float effectRange;
    public Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        addLineComponentOnce = true;
        active = false;
    }

    // Update is called once per frame
    void Update()
    {

        if(walkingScript.ability2Active)
        {
            if (addLineComponentOnce)
            {
                addLineComponentOnce = false;
                gameObject.AddComponent<LineRenderer>();
            }

            gameObject.DrawCircle(maximumRange * 6, .05f);

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
                        targetPosition = meshHit.point;
                        decoy = Instantiate(decoyPrefab, spawnPoint, attackPoint.rotation);

                        ammunition--;
                    }
                }
            }
        } else
        {
            addLineComponentOnce = true;
        }

        Collider[] pickableDecoys = Physics.OverlapSphere(transform.position, 3.0f, whatIsDecoy);

        for (int i = 0; i < pickableDecoys.Length; i++)
        {
            ammunition++;
            Destroy(pickableDecoys[i].gameObject);
        }

    }
    void OnGUI()
    {
        if (walkingScript.ability2Active) GUI.Box(new Rect(0, Screen.height - 25, 150, 25), "Decoy Active");
    }

}
