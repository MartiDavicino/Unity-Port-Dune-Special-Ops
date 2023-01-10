using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class SendDeath : MonoBehaviour
{

    public GameObject Zhib;
    public UploadController uploader;
    // Start is called before the first frame update
    public void Send(int x,int z)
    {
        StartCoroutine(uploader.SendDeath(x, z));
    }

    void OnEnable()
    {
        UploadController.OnDeath += Send;
    }

    void OnDisable()
    {
        UploadController.OnDeath -= Send;
    }
}
