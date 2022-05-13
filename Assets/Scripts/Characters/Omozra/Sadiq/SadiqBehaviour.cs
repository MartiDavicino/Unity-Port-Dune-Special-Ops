using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public enum SadiqState
{
    IDLE,
    DEVOURING,
    SPITTING,
    COMINGOUT,
    GETTINGIN,
    NONE
}
public class SadiqBehaviour : MonoBehaviour
{
    [HideInInspector] public SadiqState state;

    public GameObject omozra;
    private CharacterBaseBehavior omozraBase;
    private Vector3 omozraPos;
    
    public NavMeshAgent agent;
    public Vector3 followOffset;

    private bool abilityActive;
    [HideInInspector] public bool ability1Active;
    [HideInInspector] public bool ability2Active;
    [HideInInspector] public bool ability3Active;


    // Start is called before the first frame update
    void Start()
    {
        omozraBase = omozra.GetComponent<CharacterBaseBehavior>();

        state = SadiqState.IDLE;
        omozraPos = omozra.transform.position;

        abilityActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(ability1Active || ability2Active || ability3Active)
        {
            abilityActive = true;
            agent.ResetPath();
        } else
        {
            abilityActive = false;
        }

        if (!abilityActive)
        {
            state = SadiqState.IDLE;

            omozraPos = omozra.transform.position;
            Vector3 destination = omozraPos + (omozra.transform.rotation * followOffset);
            destination.y = 0;
            agent.SetDestination(destination);

            transform.LookAt(omozraPos);
            transform.rotation *= Quaternion.Euler(0, 180, 0);
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
