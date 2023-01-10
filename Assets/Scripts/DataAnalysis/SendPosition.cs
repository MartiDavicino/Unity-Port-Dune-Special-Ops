using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPosition : MonoBehaviour
{
    public UploadController uploader;

    // Start is called before the first frame update
    void OnDisable()
    {
        UploadController.OnMovement -= SendPos_;
    }
    void OnEnable()
    {
        UploadController.OnMovement += SendPos_;
    }

    void SendPos_(int x, int z)
    {
        StartCoroutine(uploader.SendPos(x, z));
    }

}
