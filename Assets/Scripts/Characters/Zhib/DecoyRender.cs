using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoyRender : MonoBehaviour
{

    public float circleRadius;

    public int numSegments = 128;


    Decoy decoy;

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = GameObject.Find("Decoy(Clone)");
        decoy = go.GetComponent<Decoy>();

        circleRadius = decoy.soundRange;

        gameObject.AddComponent<LineRenderer>();
        
    }
    void Update()
    {
        gameObject.DrawCircleScaled(circleRadius, 0.05f, transform.localScale);
        gameObject.transform.rotation = Quaternion.identity;

    }
   

}
