using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendKill : MonoBehaviour
{
    public GameObject Zhib;
    public GameObject uploader;
    // Start is called before the first frame update
    public void SendKill_(int x, int z)
    {
        StartCoroutine(uploader.GetComponent<UploadController>().SendKill(x, z));

    }

    void OnEnable()
    {
        uploader.GetComponent<UploadController>().OnDeath += SendKill_;
    }

    void OnDisable()
    {
        uploader.GetComponent<UploadController>().OnDeath -= SendKill_;
    }
}
