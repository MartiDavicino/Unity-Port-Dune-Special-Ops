using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualDebug : MonoBehaviour
{
    private bool once = true;
    public LineRenderer lDrawer;
    public EnemyDetection data;
    public GameObject info;

    // Start is called before the first frame update
    public void Start()
    {
        EnemyDetection data = gameObject.GetComponentInParent<EnemyDetection>();
    }
    public void Update()
    {
        if (data.debug)
        {
            if (once && gameObject.GetComponent<LineRenderer>() == null)
            {
                gameObject.AddComponent<LineRenderer>();
                lDrawer = gameObject.GetComponent<LineRenderer>();
                once = false;
            }

            gameObject.DrawCircleScaled(data.viewRadius, 0.05f, info.transform.localScale);
        }
        else
        {
            if (gameObject.GetComponent<LineRenderer>() != null)
            {
                Destroy(gameObject.GetComponent<LineRenderer>());
                once = true;
            }
        }
    }
}
