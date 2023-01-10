using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class SendDeath : MonoBehaviour
{

    public GameObject Zhib;
    // Start is called before the first frame update
    public void Send(int x,int z)
    {
        StartCoroutine(UploadController.instance.SendDeath(x, z));
    }

    void OnEnable()
    {
        UploadController.instance.OnDeath += Send;
    }

    void OnDisable()
    {
        UploadController.instance.OnDeath -= Send;
    }
}
