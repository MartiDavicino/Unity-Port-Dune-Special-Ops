using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPosition : MonoBehaviour
{

    // Start is called before the first frame update
    void OnDisable()
    {
        UploadController.instance.OnMovement -= SendPos_;
    }
    void OnEnable()
    {
        UploadController.instance.OnMovement += SendPos_;
    }

    void SendPos_(int x, int z)
    {
        StartCoroutine(UploadController.instance.SendPos(x, z));
    }

}
