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

    private GameObject enemyTarget;
    private Vector3 distanceToTarget;
    private bool attacking;

    public float rangeToKill;

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
        
        if(baseScript.selectedCharacter)
        {
            if(!baseScript.abilityActive)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out rayHit))
                    {
                        if (rayHit.collider.tag == "Enemy")
                        {
                            attacking = true;

                            enemyTarget = rayHit.collider.gameObject;

                            agent.SetDestination(rayHit.collider.gameObject.transform.position);
                        } 
                    }
                }

                if (attacking)
                {
                    distanceToTarget = CalculateAbsoluteDistance(enemyTarget.transform.position);

                    if (distanceToTarget.magnitude <= rangeToKill)
                    {
                        baseScript.state = PlayerState.STEALTH_KILL;
                        attacking = false;
                        Destroy(enemyTarget);
                    }
                }

            } else
            {
                if (attacking)
                {
                    baseScript.state = PlayerState.IDLE;
                    agent.ResetPath();
                }
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
}
