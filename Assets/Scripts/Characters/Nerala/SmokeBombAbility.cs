using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBombAbility : MonoBehaviour
{

    public CharacterBaseBehavior baseScript;

    //General Variables
    public Camera playerCamera;
    public Transform attackPoint;
    public Vector3 attackPointOffset;

    private GameObject smokeBomb;
    private bool active;
    private bool addLineComponentOnce;

    private bool bombThrown;

    //Ability Stats
    public float maximumRange;
    public int ammunition;

    //Decoy
    public LayerMask whatIsDecoy;
    public GameObject smokeBombPrefab;
    public Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        bombThrown = false;
        addLineComponentOnce = true;
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (bombThrown || baseScript.state == PlayerState.ABILITY2)
        {
            baseScript.state = PlayerState.IDLE;
            bombThrown = false;
        }

        if (baseScript.selectedCharacter)
        {
            if(baseScript.ability2Active)
            {
                if (addLineComponentOnce)
                {
                    addLineComponentOnce = false;
                    gameObject.AddComponent<LineRenderer>();
                }

                gameObject.DrawCircle(maximumRange * 10, .05f);

                if (Input.GetKeyDown(KeyCode.Mouse0) && ammunition > 0)
                {
                    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit meshHit;

                    if (Physics.Raycast(ray, out meshHit))
                    {
                        Vector3 tempDistance = CalculateAbsoluteDistance(meshHit.point);
                        if (tempDistance.magnitude > maximumRange)
                            return;

                        if (meshHit.collider.tag == "Floor")
                        {

                            baseScript.state = PlayerState.ABILITY2;
                            bombThrown = true;

                            transform.LookAt(meshHit.point);

                            Vector3 spawnPoint = attackPoint.position + (attackPoint.rotation * attackPointOffset);
                            targetPosition = meshHit.point;
                            
                            smokeBomb = Instantiate(smokeBombPrefab, spawnPoint, Quaternion.identity);  

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
        } else
        {
            addLineComponentOnce = true;
        }

    }
    Vector3 CalculateAbsoluteDistance(Vector3 targetPos)
    {
        Vector3 distance = new Vector3(0f, 0f, 0f);

        distance.x = Mathf.Abs(transform.position.x - targetPos.x);
        distance.z = Mathf.Abs(transform.position.z - targetPos.z);

        return distance;
    }
    void OnGUI()
    {
        if (baseScript.selectedCharacter)
            if (baseScript.ability2Active)
                GUI.Box(new Rect(0, Screen.height - 25, 150, 25), "Smoke Bomb Active");
    }

}
