using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SendDeath : MonoBehaviour
{

    // Start is called before the first frame update
    void OnEnable()
    {
        UploadController.instance.OnDeath += SendDeath_;
    }


    public void SendDeath_(int x, int z)
    {
        StartCoroutine(UploadController.instance.SendDeath(x, z));

    }
}
