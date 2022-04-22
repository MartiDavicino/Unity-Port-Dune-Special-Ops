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

        triggerZone.radius = circleRadius;

        gameObject.AddComponent<LineRenderer>();
        DoRenderer();
    }
    void Update()
    {
        gameObject.transform.rotation = Quaternion.identity;

    }
    void DoRenderer()
    {
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        Color c1 = new Color(0.5f, 0.5f, 0.5f, 1);
        lineRenderer.SetColors(c1, c1);
        lineRenderer.SetWidth(0.1f, 0.1f);
        lineRenderer.SetVertexCount(numSegments + 1);
        lineRenderer.useWorldSpace = false;
        lineRenderer.alignment = LineAlignment.View;

        float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
        float theta = 0f;

        for (int i = 0; i < numSegments + 1; i++)
        {
            float x = circleRadius * 1f * Mathf.Cos(theta);
            float z = circleRadius * 1f * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, 0, z);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
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
