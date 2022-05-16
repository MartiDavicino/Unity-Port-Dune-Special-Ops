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

        if(!baseScript.isTired)
            percent = baseScript.staminaTimer / baseScript.staminaSeconds;
        else
            percent = baseScript.staminaTimer / baseScript.recoveryTime;


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
    }
}
