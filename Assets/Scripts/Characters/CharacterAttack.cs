using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    private CharacterBaseBehavior baseScript;
    private RaycastHit rayHit;
    private NavMeshAgent agent;
    private Camera playerCamera;

    // private EnemyBehaviour enemyscript;
    // public EnemyState targetState;

    private GameObject enemyTarget;
    private Vector3 distanceToTarget;
    private bool attacking;
    private bool hasAttacked;

    public float soundRange;
    public float rangeToKill;
    public LayerMask whatIsEnemy;
    public GameObject spicePrefab;

    // Start is called before the first frame update
    void Start()
    {
        attacking = false;
        playerCamera = Camera.main;
        baseScript = gameObject.GetComponent<CharacterBaseBehavior>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAttacked || baseScript.state == PlayerState.STEALTH_KILL)
        {
            baseScript.state = PlayerState.IDLE;
            hasAttacked = false;
        }

        if (baseScript.selectedCharacter)
        {
            if(!baseScript.abilityActive)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out rayHit, 1000, LayerMask.GetMask("Enemy")))
                    {
                        if (rayHit.collider.tag == "Enemy")
                        {
                            attacking = true;

                            enemyTarget = rayHit.collider.gameObject;
                            agent.SetDestination(rayHit.collider.gameObject.transform.position);
                        } 
                    }
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out rayHit, 1000, LayerMask.GetMask("Enemy")))
                    {
                        if (rayHit.collider.tag != "Enemy")
                        {
                            attacking = false;
                        }
                    }
                }

                if (attacking)
                {
                    distanceToTarget = CalculateAbsoluteDistance(enemyTarget.transform.position);
                    agent.SetDestination(enemyTarget.transform.position);

                    if (distanceToTarget.magnitude <= rangeToKill)
                    {
                        hasAttacked = true;
                        baseScript.state = PlayerState.STEALTH_KILL;
                        attacking = false;
                        Attack(enemyTarget);
                    
                    }
                }
            } else
            {
                if (attacking)
                {
                    baseScript.state = PlayerState.IDLE;
                    agent.ResetPath();
                }

                hasAttacked = false;
                attacking = false;
            }
        }
    }
    Vector3 CalculateAbsoluteDistance(Vector3 targetPos)
    {
        Vector3 distance = new Vector3(0f, 0f, 0f);

        distance.x = Mathf.Abs(transform.position.x - targetPos.x);
        distance.z = Mathf.Abs(transform.position.z - targetPos.z);

        return distance;
    }

    void Attack(GameObject enemyTarget)
    {
        EnemyBehaviour eBehaviour = enemyTarget.GetComponent<EnemyBehaviour>();
        EnemyDetection eDetection = enemyTarget.GetComponent<EnemyDetection>();

        switch (gameObject.name)
        {
            case "Zhib":
                switch(eBehaviour.type)
                {
                    case EnemyType.HARKONNEN:
                        switch (eDetection.state)
                        {
                            case DecState.STILL:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                break;

                            case DecState.SEEKING:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                break;

                            case DecState.FOUND:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                baseScript.hit = true;
                                EmitSound();
                                break;
                        }
                        break;
                    case EnemyType.SARDAUKAR:
                        switch (eDetection.state)
                        {
                            case DecState.STILL:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                break;

                            case DecState.SEEKING:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                break;

                            case DecState.FOUND:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                baseScript.hit = true;
                                EmitSound();
                                break;
                        }
                        break;
                    case EnemyType.MENTAT:
                        switch (eDetection.state)
                        {
                            case DecState.STILL:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                break;

                            case DecState.SEEKING:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                break;

                            case DecState.FOUND:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                EmitSound();
                                break;
                        }
                        break;
                }
                break;
            case "Nerala":
                switch (eBehaviour.type)
                {
                    case EnemyType.HARKONNEN:
                        switch (eDetection.state)
                        {
                            case DecState.STILL:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                break;

                            case DecState.SEEKING:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                break;

                            case DecState.FOUND:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                baseScript.hit = true;
                                EmitSound();
                                break;
                        }
                        break;
                    case EnemyType.SARDAUKAR:
                        switch (eDetection.state)
                        {
                            case DecState.STILL:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                break;

                            case DecState.SEEKING:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                break;

                            case DecState.FOUND:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                baseScript.hit = true;
                                EmitSound();
                                break;
                        }
                        break;
                    case EnemyType.MENTAT:
                        switch (eDetection.state)
                        {
                            case DecState.STILL:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                break;

                            case DecState.SEEKING:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                break;

                            case DecState.FOUND:
                                SpawnSpice(eBehaviour, spicePrefab, enemyTarget.transform.position, enemyTarget.transform.rotation);
                                Destroy(enemyTarget);
                                EmitSound();
                                break;
                        }
                        break;
                }
                break;
        }
    }

    void SpawnSpice(EnemyBehaviour eBehaviour, GameObject spicePrefab, Vector3 pos, Quaternion rot)
    {
        switch (eBehaviour.type)
        {
            case EnemyType.HARKONNEN:
                if (Random.value < eBehaviour.harkonnenDropChance)
                {
                    GameObject spiceDropped = Instantiate(spicePrefab, pos, rot);
                    Spice spiceScript = spiceDropped.GetComponent<Spice>();
                    spiceScript.spiceAmmount = Random.Range(eBehaviour.harkonnenMinDrop, eBehaviour.harkonnenMaxDrop);
                }
                break;
            case EnemyType.SARDAUKAR:
                if (Random.value < eBehaviour.sardaukarDropChance)
                {
                    GameObject spiceDropped = Instantiate(spicePrefab, pos, rot);
                    Spice spiceScript = spiceDropped.GetComponent<Spice>();
                    spiceScript.spiceAmmount = Random.Range(eBehaviour.sardaukarMinDrop, eBehaviour.sardaukarMaxDrop);
                }
                break;
            case EnemyType.MENTAT:
                if (Random.value < eBehaviour.mentatDropChance)
                {
                    Quaternion spawnRot = rot;
                    spawnRot.x = 90;
                    GameObject spiceDropped = Instantiate(spicePrefab, pos, spawnRot);
                    spiceDropped.GetComponent<Spice>().spiceAmmount = Random.Range(eBehaviour.mentatMinDrop, eBehaviour.mentatMaxDrop); ;
                }
                break;
        }
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
