using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HunterSeeker : MonoBehaviour
{
    private NavMeshAgent playerAgent;

    private GameObject nerala;
    private HunterSeekerAbility baseScript;

    private float countdownTime;
    private float elapse_time;

    private CharacterBaseBehavior chBaseScript;

    private Camera playerCamera;
    private CameraMovement cameraScript;
    public LayerMask whatIsEnemy;

    // Start is called before the first frame update
    void Start()
    {
        playerAgent = GetComponent<NavMeshAgent>();

        nerala = GameObject.Find("Nerala");
        baseScript = nerala.GetComponent<HunterSeekerAbility>();
        countdownTime = baseScript.countdownTime;
        elapse_time = 0f;
        playerAgent.speed = baseScript.hunterSeekerVelocity;
        playerCamera = Camera.main;
        cameraScript = playerCamera.GetComponent<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {

            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit meshHit;

            if (Physics.Raycast(ray, out meshHit))
            {
                if (meshHit.collider.tag == "Floor")
                {
                    playerAgent.SetDestination(meshHit.point);

                }
            }
        }

        Collider[] killableEnemy = Physics.OverlapSphere(transform.position, 1.0f, whatIsEnemy);

        for (int i = 0; i < killableEnemy.Length; i++)
        {
            DisableHunterSeeker();
            Destroy(killableEnemy[i].gameObject);
            
            break;
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
}
