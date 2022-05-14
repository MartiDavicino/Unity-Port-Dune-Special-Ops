using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ÑamÑamAbility : MonoBehaviour
{
    private GameObject omozra;
    private OmozraÑamÑamAbility omozraÑamScript;
    private GameObject targetEnemy;

    private SadiqBehaviour baseScript;

    private float elapse_time;

    private bool once;

    private bool phase1;
    private bool phase2;

    // Start is called before the first frame update
    void Start()
    {
        omozra = GameObject.Find("Omozra");
        omozraÑamScript = omozra.GetComponent<OmozraÑamÑamAbility>();

        elapse_time = 0f;
        baseScript = gameObject.GetComponent<SadiqBehaviour>();
        once = true;

        phase1 = true;
        phase2 = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (baseScript.ability2Active)
        {
            targetEnemy = omozraÑamScript.targetEnemy;

            if (once)
            {
                baseScript.state = SadiqState.GETTINGIN;
                once = false;
            }

            if (phase1)
            {
                while (elapse_time < 1.2f)
                {
                    elapse_time += Time.deltaTime;
                    return;
                }

                elapse_time = 0;

                transform.position = targetEnemy.transform.position;
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

                DevourEnemy();

                phase2 = false;
                phase1 = false;
            }
            
            Collider cll = GetComponent<Collider>();
            
            while (elapse_time < 1.5f)
            {
                transform.position -= Vector3.up * Time.deltaTime * 5f;
                cll.isTrigger = true;
                elapse_time += Time.deltaTime;
                return;
            }

            
            elapse_time = 0;

 
            transform.position = omozra.transform.position + (omozra.transform.rotation * baseScript.followOffset);
            baseScript.state = SadiqState.IDLE;
            
            baseScript.ability2Active = false;
            omozraÑamScript.baseScript.ability2Active = false;
            omozraÑamScript.baseScript.abilityActive = false;
            omozraÑamScript.onCooldown = true;

            if(omozra.GetComponent<LineRenderer>() != null)
                Destroy(omozra.GetComponent<LineRenderer>());
            
            
            omozraÑamScript.addLineComponentOnce = true;

            cll.isTrigger = false;
        }
    }

    void DevourEnemy()
    {
        EnemyBehaviour eBehaviour = targetEnemy.GetComponent<EnemyBehaviour>();
        EnemyDetection eDetection = targetEnemy.GetComponent<EnemyDetection>();

        switch (eBehaviour.type)
        {
            case EnemyType.HARKONNEN:
                switch (eDetection.state)
                {
                    case DecState.STILL:
                        if (Random.value < omozraÑamScript.harkonnenUnaware)
                        {
                            Destroy(targetEnemy);
                        }
                        break;

                    case DecState.SEEKING:
                        if (Random.value < omozraÑamScript.harkonnenAware)
                        {
                            Destroy(targetEnemy);
                        }
                        break;

                    case DecState.FOUND:
                        if (Random.value < omozraÑamScript.harkonnenDetected)
                        {
                            Destroy(targetEnemy);
                        }
                        break;
                }
                break;
            case EnemyType.SARDAUKAR:
                switch (eDetection.state)
                {
                    case DecState.STILL:
                        if (Random.value < omozraÑamScript.sardaukarUnaware)
                        {
                            Destroy(targetEnemy);
                        }
                        break;

                    case DecState.SEEKING:
                        if (Random.value < omozraÑamScript.sardaukarAware)
                        {
                            Destroy(targetEnemy);
                        }
                        break;

                    case DecState.FOUND:
                        if (Random.value < omozraÑamScript.sardaukarDetected)
                        {
                            Destroy(targetEnemy);
                        }
                        break;
                }
                break;
            case EnemyType.MENTAT:
                switch (eDetection.state)
                {
                    case DecState.STILL:
                        if (Random.value < omozraÑamScript.mentatUnaware)
                        {
                            Destroy(targetEnemy);
                        }
                        break;

                    case DecState.SEEKING:
                        if (Random.value < omozraÑamScript.mentatAware)
                        {
                            Destroy(targetEnemy);
                        }
                        break;

                    case DecState.FOUND:
                        if (Random.value < omozraÑamScript.mentatDetected)
                        {
                            Destroy(targetEnemy);
                        }
                        break;
                }
                break;
        }
    }
}
