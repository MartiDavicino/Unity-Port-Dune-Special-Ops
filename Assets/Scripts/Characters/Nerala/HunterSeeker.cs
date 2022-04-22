using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HunterSeeker : MonoBehaviour
{
    public NavMeshAgent playerAgent;

    private Camera playerCamera;
    private CameraMovement cameraScript;
    public LayerMask whatIsEnemy;

    // Start is called before the first frame update
    void Start()
    {
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
            GameObject go = GameObject.Find("Nerala");

            HunterSeekerAbility hunterSeekerScript = go.GetComponent<HunterSeekerAbility>();
            hunterSeekerScript.seekerHunting = false;

            CharacterBaseBehavior baseScript = go.GetComponent<CharacterBaseBehavior>();
            baseScript.selectedCharacter = true;
            baseScript.abilityActive = false;
            baseScript.ability3Active = false;

            cameraScript.focusedPlayer = go;

            Destroy(killableEnemy[i].gameObject);
            Destroy(gameObject);
        }
    }
}
