using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectionMeter : MonoBehaviour
{
    [SerializeField]
    private Image bar;

    private EnemyDetection enemyD;

    private EnemyBehaviour enemyB;

    public float percent;

    private Camera playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        enemyD = gameObject.GetComponentInParent<EnemyDetection>();
        enemyB = gameObject.GetComponentInParent<EnemyBehaviour>();

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
            percent = enemyD.timer / enemyD.secondsToDetect;
            if (percent >= 1.0f)
            {
                percent = 1.0f;
            }
        
            bar.color = Color.yellow;
            bar.fillAmount = percent ;

        }
        
        if(enemyD.state == DecState.FOUND)
        {
            bar.color = Color.red;
        }
    }
}
