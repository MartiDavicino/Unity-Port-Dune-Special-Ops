using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;



public class LetsGoAbility : MonoBehaviour
{
    private GameObject omozra;
    private OmozraLetsGoAbility omozraLetsGoScript;
    private GameObject targetCharacter;

    private SadiqBehaviour baseScript;

    private Vector3 initPos;
    [HideInInspector] public Vector3 targetPos;
    [HideInInspector] public float elapse_time;

    private bool once;

    private bool phase1;
    private bool phase2;

    [HideInInspector] public bool pukePhase;

    // Start is called before the first frame update
    void Start()
    {
        omozra = GameObject.Find("Omozra");
        omozraLetsGoScript = omozra.GetComponent<OmozraLetsGoAbility>();

        initPos = transform.position;

        elapse_time = 0f;
        baseScript = gameObject.GetComponent<SadiqBehaviour>();
        once = true;

        phase1 = true;
        phase2 = false;

        pukePhase = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (baseScript.ability3Active)
        {
            targetCharacter = omozraLetsGoScript.targetCharacter;

            if (once && !pukePhase)
            {
                baseScript.state = SadiqState.GETTINGIN;
                once = false;
            }

            if (phase1)
            {
                omozraLetsGoScript.characterChosen = true;

                while (elapse_time < 1.2f)
                {
                    elapse_time += Time.deltaTime;
                    return;
                }

                elapse_time = 0;

                transform.position = targetCharacter.transform.position;
                baseScript.state = SadiqState.DEVOURING;

                phase1 = false;
                phase2 = true;
            }

            if (phase2)
            {
                while (elapse_time < 0.7f)
                {
                    elapse_time += Time.deltaTime;
                    return;
                }

                elapse_time = 0;

                Vector3 ySeFueALaPutaAllàPorLarbolà = new Vector3(-5000f, -6, -5000f);

                NavMeshAgent agent = targetCharacter.GetComponent<NavMeshAgent>();
                agent.Warp(ySeFueALaPutaAllàPorLarbolà);

                omozraLetsGoScript.characterEaten = true;

                phase2 = false;
                phase1 = false;
            }
            

            Collider cll = GetComponent<Collider>();
            
            while (elapse_time < 1.5f && !pukePhase)
            {
                transform.position -= Vector3.up * Time.deltaTime * 5f;
                cll.isTrigger = true;
                elapse_time += Time.deltaTime;
                return;
            }


            if(pukePhase)
            {
                omozraLetsGoScript.characterChosen = false;
                omozraLetsGoScript.locationChosen = true;

                transform.position = targetPos;

                baseScript.state = SadiqState.SPITTING;

                while (elapse_time < 1.7f)
                {
                    elapse_time += Time.deltaTime;
                    return;
                }

                elapse_time = 0;

                NavMeshAgent agent = targetCharacter.GetComponent<NavMeshAgent>();
                agent.Warp(transform.position);


                transform.position = omozra.transform.position + (omozra.transform.rotation * baseScript.followOffset);
                baseScript.state = SadiqState.IDLE;
            
                baseScript.ability3Active = false;
                omozraLetsGoScript.characterEaten = false;
                omozraLetsGoScript.baseScript.ability3Active = false;
                omozraLetsGoScript.baseScript.abilityActive = false;
                omozraLetsGoScript.onCooldown = true;

                if(omozra.GetComponent<LineRenderer>() != null)
                    Destroy(omozra.GetComponent<LineRenderer>());

                once = true;

                pukePhase = false;
                phase1 = true;
                phase2 = false;

                omozraLetsGoScript.addLineComponentOnce = true;

                cll.isTrigger = false;

                omozraLetsGoScript.locationChosen = false;
            }
        }
    }
}
