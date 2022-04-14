using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;


public class CameraMovement : MonoBehaviour
{
    public GameObject player;   
    private Vector3 offset;            
    public float rotSpeed = 4.0f;
    
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {

        offset = Quaternion.AngleAxis(Input.GetAxis("Rotation") * rotSpeed, Vector3.up) * offset;
        transform.position = player.transform.position + offset;
        transform.LookAt(player.transform);
    }
}