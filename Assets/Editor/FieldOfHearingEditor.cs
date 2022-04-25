using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof (FieldOfHearing))]

public class FieldOfHearingEditor : Editor
{
    void OnSceneGUI()
    {
        FieldOfHearing foh = (FieldOfHearing)target;
        Handles.color = Color.blue;
        Handles.DrawWireArc(foh.transform.position, Vector3.up, Vector3.forward, 360,foh.viewRadius);
        Handles.color = Color.cyan;
        foreach (Transform visibleTarget in foh.visibleTargets)
        {
            Handles.DrawLine(foh.transform.position, visibleTarget.position);
        }

    }
}
