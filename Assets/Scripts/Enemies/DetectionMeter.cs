using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectionMeter : MonoBehaviour
{
    [SerializeField]
    private Image bar;

    public EnemyDetection enemyD;

    public EnemyBehaviour enemyB;

    public float percent;

    private Camera playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        percent = 0.0f;
        bar.fillAmount = percent;
        bar.color = Color.green;
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        transform.LookAt(playerCamera.transform);

        if(enemyD.state == DecState.SEEKING)
        {
            percent = enemyD.timer / enemyD.delay;
            if (percent >= 1.0f)
            {
                percent = 1.0f;
            }
        
            bar.color = Color.yellow;
            bar.fillAmount = percent ;

        }
        else if(enemyD.state == DecState.FOUND)
        {
            bar.color = Color.red;
        }
        if(enemyD.state == DecState.STILL)
        {
            percent = 0.0f;
            bar.fillAmount = percent;
            bar.color = Color.green;
        }
    }
}
