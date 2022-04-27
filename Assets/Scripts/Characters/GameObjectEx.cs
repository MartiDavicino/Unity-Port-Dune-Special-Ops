using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectEx
{
    public static void DrawCircleScaled(this GameObject container, float radius, float lineWidth, Vector3 scale)
    {
            
        var line = container.GetComponent<LineRenderer>();
            
        var segments = 360;
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;

        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius /scale.x, 0, Mathf.Cos(rad) * radius / scale.z);
        }

            
        line.SetPositions(points);
    }

    public static void DrawCircle(this GameObject container, float radius, float lineWidth)
    {

        var line = container.GetComponent<LineRenderer>();

        var segments = 360;
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;

        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius , 0, Mathf.Cos(rad) * radius );
        }


        line.SetPositions(points);
    }
}