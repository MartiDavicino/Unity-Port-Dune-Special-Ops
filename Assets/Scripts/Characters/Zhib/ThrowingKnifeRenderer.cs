using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnifeRenderer : MonoBehaviour
{
    public float circleRadius;
    public int numSegments = 128;

    ThrowingKnife knife;

    private void Start()
    {
        GameObject go = GameObject.Find("Knife(Clone)");
        knife = go.GetComponent<ThrowingKnife>();

        circleRadius = knife.soundRange;

        gameObject.AddComponent<LineRenderer>();
    }

    private void Update()
    {
        gameObject.DrawCircleScaled(circleRadius, 0.05f, transform.localScale);
        gameObject.transform.rotation = Quaternion.identity;
    }
}
