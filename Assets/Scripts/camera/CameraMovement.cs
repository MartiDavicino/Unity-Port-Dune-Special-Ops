using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraMovement : MonoBehaviour
{


    public GameObject focusedPlayer;  
    
    public float rotSpeed;
    public float angle;

    private float remainingAngle;

    public Vector3 offset;            

    private GeneralManager manager;
    void Start()
    {
        transform.position = transform.position + offset;
        manager = gameObject.GetComponent<GeneralManager>();
    }

    void LateUpdate()
    {
        if(!manager.gameLost)
        {
            if(!Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown("e"))
                    remainingAngle += angle;

                if (Input.GetKeyDown("q"))
                    remainingAngle -= angle;

                float newRemainingAngle = Mathf.MoveTowards(remainingAngle, 0, rotSpeed * Time.deltaTime);
                float delta = remainingAngle - newRemainingAngle;
                remainingAngle = newRemainingAngle;
                offset = Quaternion.AngleAxis(delta, Vector3.up) * offset;
                transform.position = focusedPlayer.transform.position + offset;
                transform.LookAt(focusedPlayer.transform);
            } else if (Input.GetKey(KeyCode.LeftShift))
            {
                offset = Quaternion.AngleAxis(Input.GetAxis("Rotation") * rotSpeed * 0.001f, Vector3.up) * offset;
                transform.position = focusedPlayer.transform.position + offset;
                transform.LookAt(focusedPlayer.transform);
            }
        }
    }
}