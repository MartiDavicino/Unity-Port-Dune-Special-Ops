using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBombAbility : MonoBehaviour
{

    //General Variables
    private CharacterBaseBehavior baseScript;
    private Camera playerCamera;
    private Vector3 attackPointOffset;
    private bool addLineComponentOnce;
    private bool bombThrown;

    //Ability Stats
    public float maximumRange;
    public float smokeEffectRange;
    public float timeOfEffect;
    public int ammunition;

    //Decoy
    public GameObject smokeBombPrefab;
    public LayerMask whatIsSmokeBomb;
    [HideInInspector] public Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        attackPointOffset = new Vector3(0.8f, 1.5f, 0f);
        baseScript = GetComponent<CharacterBaseBehavior>();
        playerCamera = Camera.main;
        bombThrown = false;
        addLineComponentOnce = true;
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

                gameObject.DrawCircleScaled(maximumRange, 0.05f, transform.localScale);

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

                            Vector3 spawnPoint = transform.position + (transform.rotation * attackPointOffset);
                            targetPosition = meshHit.point;
                            
                            Instantiate(smokeBombPrefab, spawnPoint, Quaternion.identity);

                            ammunition--;
                        }
                    }
                }
            } else
            {
                addLineComponentOnce = true;
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
            {
                GUI.Box(new Rect(5, Screen.height - 30, 150, 25), "Smoke Bomb Active");
                GUI.Box(new Rect(160, Screen.height - 30, 150, 25), "Remaining Bombs:");
                GUI.Box(new Rect(315, Screen.height - 30, 30, 25), ammunition.ToString());
            }
    }

}
