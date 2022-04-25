using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(EnemyDetection))]
public class EnemyDetectionEditor : Editor
{
    // Start is called before the first frame update
    void OnSceneGUI()
    {
        EnemyDetection editor = (EnemyDetection)target;
        Handles.color = Color.red;
        Handles.DrawWireArc(editor.transform.position, Vector3.up, Vector3.forward, 360, editor.viewRadius);
        Vector3 viewAngleA = editor.DirFromAngle(-editor.viewAngle / 2, false);
        Vector3 viewAngleB = editor.DirFromAngle(editor.viewAngle / 2, false);

        Handles.DrawLine(editor.transform.position, editor.transform.position + viewAngleA * editor.viewRadius);
        Handles.DrawLine(editor.transform.position, editor.transform.position + viewAngleB * editor.viewRadius);

        Handles.color = Color.green;
        foreach (Transform visibleTarget in editor.visibleTargets)
        {
            Handles.DrawLine(editor.transform.position, visibleTarget.position);
        }


        Handles.color = Color.blue;
        Handles.DrawWireArc(editor.transform.position, Vector3.up, Vector3.forward, 360, editor.hearingRadius);
        Handles.color = Color.cyan;
        foreach (Transform noisyTarget in editor.noisyTargets)
        {
            Handles.DrawLine(editor.transform.position, noisyTarget.position);
        }
    }

}
