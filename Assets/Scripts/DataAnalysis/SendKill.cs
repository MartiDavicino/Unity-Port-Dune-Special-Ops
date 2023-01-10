using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendKill : MonoBehaviour
{
    public GameObject Zhib;
    public UploadController uploader;
    // Start is called before the first frame update
    public void SendKill_(int x, int z)
    {
        StartCoroutine(uploader.SendKill(x, z));

    }

    void OnEnable()
    {
        UploadController.OnDeath += SendKill_;
    }

    void OnDisable()
    {
        UploadController.OnDeath -= SendKill_;
    }
}
