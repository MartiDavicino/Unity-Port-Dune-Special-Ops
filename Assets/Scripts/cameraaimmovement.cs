using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraaimmovement : MonoBehaviour
{
    public float Speed = 0.5f;

    private bool pressedOnce = false;
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

        if (Input.GetKeyDown("e") && !pressedOnce)
        {
            transform.Rotate(transform.up, 45.0f);
            pressedOnce = true;
        }

        if (Input.GetKeyDown("q") && !pressedOnce)
        {
            transform.Rotate(transform.up, -45.0f);
            pressedOnce = true;
        }

        if (!Input.GetKey("e") && !Input.GetKey("q"))
            pressedOnce = false;


    }
}
