using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HunterSeeker : MonoBehaviour
{
    private NavMeshAgent playerAgent;

    private GameObject nerala;
    [HideInInspector] public HunterSeekerAbility baseScript;

    private float countdownTime;
    private float elapse_time;

    private CharacterBaseBehavior chBaseScript;

    private Camera playerCamera;
    private CameraMovement cameraScript;
    public LayerMask whatIsEnemy;
    private GameObject spicePrefab;

    // Start is called before the first frame update
    void Start()
    {
        playerAgent = GetComponent<NavMeshAgent>();


        nerala = GameObject.Find("Nerala");
        baseScript = nerala.GetComponent<HunterSeekerAbility>();
        spicePrefab = baseScript.spicePrefab;
        countdownTime = baseScript.countdownTime;
        elapse_time = 0f;
        playerAgent.speed = baseScript.hunterSeekerVelocity;
        playerCamera = Camera.main;
        cameraScript = playerCamera.GetComponent<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit meshHit;

            if (Physics.Raycast(ray, out meshHit, 1000, LayerMask.GetMask("Ground")))
            {
                if (meshHit.collider.tag == "Floor")
                {
                    playerAgent.SetDestination(meshHit.point);

                }
            }
        }

        while (elapse_time < countdownTime)
        {
            elapse_time += Time.deltaTime;
            if(CalculateAbsoluteDistance(nerala.transform.position).magnitude < baseScript.hunterSeekerMaxRange) return;
        }

        DisableHunterSeeker();
    }


    void OnGUI()
    {
        GUI.Box(new Rect(5, Screen.height - 30, 150, 25), "Remaining Fly Time:");
        GUI.Box(new Rect(160, Screen.height - 30, 40, 25), (countdownTime - elapse_time).ToString("F2"));
    }

    Vector3 CalculateAbsoluteDistance(Vector3 targetPos)
    {
        Vector3 distance = new Vector3(0f, 0f, 0f);

        distance.x = Mathf.Abs(transform.position.x - targetPos.x);
        distance.z = Mathf.Abs(transform.position.z - targetPos.z);

        return distance;
    }

    public void DisableHunterSeeker()
    {
        chBaseScript = nerala.GetComponent<CharacterBaseBehavior>();
        chBaseScript.selectedCharacter = true;
        chBaseScript.hunterSeeking = false;
        chBaseScript.abilityActive = false;
        chBaseScript.ability3Active = false;

        Destroy(nerala.GetComponent<LineRenderer>());

        cameraScript.focusedPlayer = nerala;

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            DisableHunterSeeker();
            EnemyBehaviour eBehaviour = collision.gameObject.GetComponent<EnemyBehaviour>();

            SpawnSpice(eBehaviour, spicePrefab, collision.gameObject.transform.position, collision.gameObject.transform.rotation);
            Destroy(collision.gameObject);
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
}
