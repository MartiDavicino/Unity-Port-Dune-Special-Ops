using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBombRender : MonoBehaviour
{

    public float circleRadius;

    public int numSegments = 128;

    public SphereCollider triggerZone;

    SmokeBomb smokeBomb;

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = GameObject.Find("SmokeBomb(Clone)");
        smokeBomb = go.GetComponent<SmokeBomb>();

        circleRadius = smokeBomb.smokeRange;

        //triggerZone.radius = circleRadius / transform.localScale;


        gameObject.AddComponent<LineRenderer>();


    }
    

    void Update()
    {
        gameObject.DrawCircleScaled(circleRadius, 0.05f, transform.localScale);
        gameObject.transform.rotation = Quaternion.identity;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SmokeBomb")
            return;

        if (smokeBomb.groundHit)
            if (other.gameObject.layer == 6)
                other.gameObject.layer = 11;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "SmokeBomb")
            return;

        if (smokeBomb.groundHit)
            if (other.gameObject.layer == 11)
                other.gameObject.layer = 6;
    }
}
