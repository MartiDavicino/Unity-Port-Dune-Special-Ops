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

    private bool onCooldown;
    private float elapse_time;
    private bool hasShot;

    //Ability Stats
    public float debuffMultiplier;
    public float maximumRange;
    public float soundRange;
    public float cooldown;

    public LayerMask whatIsEnemy;

    [Header("- Chances To Sleep -")]

    [Header("- Harkonnen -")]
    [Range(0.0f, 1.0f)]
    public float harkonnenUnaware;
    [Range(0.0f, 1.0f)]
    public float harkonnenAware;
    [Range(0.0f, 1.0f)]
    public float harkonnenDetected;

    [Header("- Sardaukar -")]
    [Range(0.0f, 1.0f)]
    public float sardaukarUnaware;
    [Range(0.0f, 1.0f)]
    public float sardaukarAware;
    [Range(0.0f, 1.0f)]
    public float sardaukarDetected;

    [Header("- Mentat -")]
    [Range(0.0f, 1.0f)]
    public float mentatUnaware;
    [Range(0.0f, 1.0f)]
    public float mentatAware;
    [Range(0.0f, 1.0f)]
    public float mentatDetected;


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

                if (Input.GetKeyDown(KeyCode.Mouse0))
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

                                ApplyPoison();

                                hasShot = true;
                                onCooldown = true;
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

                        ApplyPoison();

                        hasShot = true;
                        onCooldown = true;

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

    void ApplyPoison()
    {


        EnemyBehaviour eBehaviour = targetEnemy.GetComponent<EnemyBehaviour>();
        EnemyDetection eDetection = targetEnemy.GetComponent<EnemyDetection>();

        switch (eBehaviour.type)
        {
            case EnemyType.HARKONNEN:
                switch (eDetection.state)
                {
                    case DecState.STILL:
                        if (Random.value <= harkonnenUnaware)
                        {
                            eDetection.debuffMultiplier = debuffMultiplier;
                            eDetection.debuffed = true;
                        }
                        else
                        {
                            EmitSound();
                        }
                        break;

                    case DecState.SEEKING:
                        if (Random.value <= harkonnenAware)
                        {
                            eDetection.debuffMultiplier = debuffMultiplier;
                            eDetection.debuffed = true;
                        }
                        else
                        {
                            EmitSound();
                        }
                        break;

                    case DecState.FOUND:
                        if (Random.value <= harkonnenDetected)
                        {
                            eDetection.debuffMultiplier = debuffMultiplier;
                            eDetection.debuffed = true;
                        }
                        else
                        {
                            EmitSound();
                        }
                        break;
                }
                break;
            case EnemyType.SARDAUKAR:
                switch (eDetection.state)
                {
                    case DecState.STILL:
                        if (Random.value <= sardaukarUnaware)
                        {
                            eDetection.debuffMultiplier = debuffMultiplier;
                            eDetection.debuffed = true;
                        }
                        else
                        {
                            EmitSound();
                        }
                        break;

                    case DecState.SEEKING:
                        if (Random.value <= sardaukarAware)
                        {
                            eDetection.debuffMultiplier = debuffMultiplier;
                            eDetection.debuffed = true;
                        }
                        else
                        {
                            EmitSound();
                        }
                        break;

                    case DecState.FOUND:
                        if (Random.value <= sardaukarDetected)
                        {
                            eDetection.debuffMultiplier = debuffMultiplier;
                            eDetection.debuffed = true;
                        }
                        else
                        {
                            EmitSound();
                        }
                        break;
                }
                break;
            case EnemyType.MENTAT:
                switch (eDetection.state)
                {
                    case DecState.STILL:
                        if (Random.value <= mentatUnaware)
                        {
                            eDetection.debuffMultiplier = debuffMultiplier;
                            eDetection.debuffed = true;
                        }
                        else
                        {
                            EmitSound();
                        }
                        break;

                    case DecState.SEEKING:
                        if (Random.value <= mentatAware)
                        {
                            eDetection.debuffMultiplier = debuffMultiplier;
                            eDetection.debuffed = true;
                        }
                        else
                        {
                            EmitSound();
                        }
                        break;

                    case DecState.FOUND:
                        if (Random.value <= mentatDetected)
                        {
                            eDetection.debuffMultiplier = debuffMultiplier;
                            eDetection.debuffed = true;
                        }
                        else
                        {
                            EmitSound();
                        }
                        break;
                }
                break;
        }
    }

    Vector3 CalculateAbsoluteDistance(Vector3 targetPos)
    {
        Vector3 distance = new Vector3(0f, 0f, 0f);

        distance.x = Mathf.Abs(transform.position.x - targetPos.x);
        distance.z = Mathf.Abs(transform.position.z - targetPos.z);

        return distance;
    }

    void EmitSound()
    {
        Collider[] affectedEnemies = Physics.OverlapSphere(transform.position, soundRange, whatIsEnemy);

        for (int i = 0; i < affectedEnemies.Length; i++)
        {
            affectedEnemies[i].GetComponent<EnemyDetection>().state = DecState.SEEKING;
            affectedEnemies[i].GetComponent<EnemyDetection>().timer = affectedEnemies[i].GetComponent<EnemyDetection>().secondsPerBar;

        }
    }
}
