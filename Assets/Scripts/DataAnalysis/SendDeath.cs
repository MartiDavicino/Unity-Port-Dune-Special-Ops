using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class SendDeath : MonoBehaviour
{

    public GameObject Zhib;
    public GameObject uploader;
    // Start is called before the first frame update
    public void Send(int x,int z)
    {
        StartCoroutine(uploader.GetComponent<UploadController>().SendDeath(x, z));
    }

    void OnEnable()
    {
        uploader.GetComponent<UploadController>().OnDeath += Send;
    }

    void OnDisable()
    {
        uploader.GetComponent<UploadController>().OnDeath -= Send;
    }
}
