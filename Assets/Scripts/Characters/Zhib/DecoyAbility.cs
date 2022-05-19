using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DecoyAbility : MonoBehaviour
{

    private CharacterBaseBehavior baseScript;

    //General Variables
    private Camera playerCamera;
    private Vector3 attackPointOffset;

    private bool decoyThrown;
    private bool addLineComponentOnce;

    //Ability Stats
    public float maximumRange;

    //Decoy
    public float effectRange;

    public float cooldown;
    private bool onCooldown;
    private float elapse_time;
    private bool startTimer;

    public GameObject decoyPrefab;
    public LayerMask whatIsDecoy;

    [HideInInspector] public Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = GetComponent<CharacterBaseBehavior>();

        playerCamera = Camera.main;
        attackPointOffset = new Vector3(0.8f, 1.5f, 0);

        elapse_time = 0f;
        startTimer = false;

        decoyThrown = false;
        addLineComponentOnce = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (decoyThrown || baseScript.state == PlayerState.ABILITY2)
        {
            baseScript.state = PlayerState.IDLE;
            decoyThrown = false;
        }

        if (baseScript.selectedCharacter)
        {
            if(baseScript.ability2Active && !onCooldown)
            {
                if (addLineComponentOnce)
                {
                    addLineComponentOnce = false;
                    gameObject.AddComponent<LineRenderer>();
                }

                gameObject.DrawCircleScaled(maximumRange, 0.05f, transform.localScale);

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {

                    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit meshHit;

                    if (Physics.Raycast(ray, out meshHit, 1000, LayerMask.GetMask("Ground")))
                    {
                        Vector3 tempDistance = CalculateAbsoluteDistance(meshHit.point);
                        if (tempDistance.magnitude > maximumRange)
                            return;

                        if (meshHit.collider.tag == "Floor")
                        {
                            decoyThrown = true;
                            baseScript.state = PlayerState.ABILITY2;

                            transform.LookAt(meshHit.point);

                            Vector3 spawnPoint = transform.position + (transform.rotation * attackPointOffset);
                            targetPosition = meshHit.point;
                            Instantiate(decoyPrefab, spawnPoint, transform.rotation);

                            onCooldown = true;

                        }
                    }
                }
            } else
            {
                addLineComponentOnce = true;
            }

            Collider[] pickables = Physics.OverlapSphere(transform.position, 3.0f, whatIsDecoy);

            for (int i = 0; i < pickables.Length; i++)
            {
                if (pickables[i].gameObject.tag == "Decoy")
                {
                    Destroy(pickables[i].gameObject);
                    startTimer = true;
                }
            }
        } else
        {
            addLineComponentOnce = true;
        }

        if(onCooldown)
        {
            if(startTimer)
            {
                while (elapse_time < cooldown)
                {
                    elapse_time += Time.deltaTime;
                    return;
                }

                elapse_time = 0;
                onCooldown = false;
                startTimer = false;
            }
        }
    }

    void OnGUI()
    {
        if (baseScript.selectedCharacter)
            if (baseScript.ability2Active)
            {
                GUI.Box(new Rect(5, Screen.height - 30, 150, 25), "Decoy Active");

                if(onCooldown)
                    GUI.Box(new Rect(160, Screen.height - 30, 40, 25), (cooldown - elapse_time).ToString("F2"));
            }
    }

    Vector3 CalculateAbsoluteDistance(Vector3 targetPos)
    {
        Vector3 distance = new Vector3(0f, 0f, 0f);

        distance.x = Mathf.Abs(transform.position.x - targetPos.x);
        distance.z = Mathf.Abs(transform.position.z - targetPos.z);

        return distance;
    }

}
