using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    [SerializeField]
    private Image bar;

    private CharacterBaseBehavior baseScript;

    public float percent;

    private Camera playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = gameObject.GetComponentInParent<CharacterBaseBehavior>();

        percent = 0.0f;
        bar.fillAmount = percent;
        bar.color = Color.green;
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        transform.LookAt(playerCamera.transform);

        percent = baseScript.staminaTimer / baseScript.staminaSeconds;
        
        bar.enabled = true;

        if (percent >= 1.0f)
        {
            percent = 1.0f;
            bar.enabled = false;
        }

        if(percent < 1 && percent > 0.5)
        {
            bar.color = Color.yellow;
            bar.fillAmount = percent;
        }

        if (percent <= 0.5)
        {
            Color orange = new Color(1.0f, 0.55f, 0.0f);

            bar.color = orange;
            bar.fillAmount = percent;
        }

        if(baseScript.isTired)
        {
            bar.color = Color.red;
        }

        //if (baseScript.staminaTimer == 0)
        //{
        //    bar.fillAmount = 0f;
        //    return;
        //}

        //if (enemyD.state == DecState.STILL)
        //{
        //    percent = enemyD.timer / enemyD.secondsPerBar;
        //    if (percent >= 1.0f)
        //    {
        //        percent = 1.0f;
        //    }

        //    bar.color = Color.green;
        //    bar.fillAmount = percent;
        //}

        //if (enemyD.state == DecState.SEEKING)
        //{
        //    percent = enemyD.timer / enemyD.secondsPerBar;
        //    if (percent >= 1.0f)
        //    {
        //        percent = 1.0f;
        //    }

        //    Color orange = new Color(1.0f, 0.64f, 0.0f);

        //    bar.color = orange;
        //    bar.fillAmount = percent;
        //}

        //if (enemyD.state == DecState.FOUND)
        //{
        //    if (enemyB.waveSpawned)
        //        bar.color = Color.white;
        //    else if (enemyB.type == EnemyType.MENTAT)
        //        bar.color = Color.blue;
        //    else
        //        bar.color = Color.red;
        //}
    }
}
