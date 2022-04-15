using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraaimmovement : MonoBehaviour
{
    public float Speed = 0.5f;
    //public float rotSpeed = 4.0f;

    void Update()
    {
        float xAxisValue = Input.GetAxis("Horizontal") * Speed;
        float zAxisValue = Input.GetAxis("Vertical") * Speed;
        float yAxisValue = Input.GetAxis("Jump") * Speed;
        //float Rotation = Input.GetAxis("Rotation") * rotSpeed;

        transform.position += transform.forward * zAxisValue;
        transform.position += transform.up * yAxisValue;
        transform.position += transform.right * xAxisValue;
        
        //transform.Rotate(transform.up * Rotation);
    }
}
