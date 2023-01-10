using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPosition : MonoBehaviour
{
    public GameObject uploader;

    // Start is called before the first frame update
    void OnDisable()
    {
        uploader.GetComponent<UploadController>().OnMovement -= SendPos_;
    }
    void OnEnable()
    {
        Debug.Log("AAA");
        uploader.GetComponent<UploadController>().OnMovement += SendPos_;
    }

    void SendPos_(int x, int z)
    {
        StartCoroutine(uploader.GetComponent<UploadController>().SendPos(x, z));
    }

}
