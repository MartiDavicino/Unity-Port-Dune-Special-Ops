using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


public class SleepingDartAbility : MonoBehaviour
{

    //General Variables
    private CharacterBaseBehavior baseScript;
    private Camera playerCamera;
    private NavMeshAgent agent;
    private RaycastHit rayHit;
    private bool enemyOutOfRange;
    private bool addLineComponentOnce;

    private GameObject targetEnemy;
    private Vector3 targetDistance;

    //Ability Stats
    public float sightDebuffMultiplier;
    public float maximumRange;

    public float cooldown;
    private bool onCooldown;
    private float elapse_time;

    public int ammunition;

    private bool hasShot;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = GetComponent<CharacterBaseBehavior>();
        playerCamera = Camera.main;

        onCooldown = false;
        elapse_time = 0f;

        hasShot = false;
        addLineComponentOnce = true;
        enemyOutOfRange = false;

        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasShot || baseScript.state == PlayerState.ABILITY1)
        {
            baseScript.state = PlayerState.IDLE;
            hasShot = false;
        }

        if (baseScript.selectedCharacter)
        {
            if (baseScript.ability1Active && !onCooldown)
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

                    if (Physics.Raycast(ray, out rayHit))
                    {
                        if (rayHit.collider.tag == "Enemy")
                        {
                            targetEnemy = rayHit.collider.gameObject;
                            targetDistance = CalculateAbsoluteDistance(rayHit.point);

                            if (targetDistance.magnitude >= maximumRange)
                            {
                                enemyOutOfRange = true;
                                agent.SetDestination(targetEnemy.transform.position);

                                baseScript.state = PlayerState.WALKING;

                            } else
                            {
                                gameObject.transform.LookAt(targetEnemy.transform);
                                gameObject.transform.rotation *= Quaternion.Euler(0, 90, 0);

                                baseScript.state = PlayerState.ABILITY1;

                                // Set Sleepy Effect to Enemy 
                                EnemyDetection eD = targetEnemy.GetComponent<EnemyDetection>();
                                eD.multiplierHolder *= sightDebuffMultiplier;

 
                                //

                                hasShot = true;
                                onCooldown = true;
                                ammunition--;
                            }
                        }
                    }
                }

                if (enemyOutOfRange)
                {
                    targetDistance = CalculateAbsoluteDistance(targetEnemy.transform.position);

                    if (targetDistance.magnitude <= maximumRange)
                    {
                        agent.ResetPath();

                        gameObject.transform.LookAt(targetEnemy.transform);

                        gameObject.transform.rotation *= Quaternion.Euler(0, -180, 0);

                        baseScript.state = PlayerState.ABILITY1;

                        // Set Sleepy Effect to Enemy 
                        EnemyDetection eD = targetEnemy.GetComponent<EnemyDetection>();
                        eD.multiplierHolder *= sightDebuffMultiplier;

                        //

                        hasShot = true;
                        onCooldown = true;

                        ammunition--;
                        enemyOutOfRange = false;

                    }
                }

            } else
            {
                enemyOutOfRange = false;
                addLineComponentOnce = true;
            }
        } else
        {
            addLineComponentOnce = true;
        }

        if (onCooldown)
        {
            while (elapse_time < cooldown)
            {
                elapse_time += Time.deltaTime;
                return;
            }

            elapse_time = 0;
            onCooldown = false;
        }
    }
    void OnGUI()
    {
        if (baseScript.selectedCharacter)
            if (baseScript.ability1Active)
            {
                GUI.Box(new Rect(5, Screen.height - 30, 150, 25), "Sleeping Dart Active");

                if (onCooldown)
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
