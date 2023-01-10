using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class SendDeath : MonoBehaviour
{

    // Start is called before the first frame update
    void OnEnable()
    {
        UploadController.instance.OnDeath += Send;
    }

    void OnDisable()
    {
        UploadController.instance.OnDeath -= Send;
    }
    void Send(int x, int z)
    {
        StartCoroutine(UploadController.instance.SendDeath(x, z));
    }
}
