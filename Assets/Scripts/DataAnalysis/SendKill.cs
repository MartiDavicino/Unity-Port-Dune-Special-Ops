using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendKill : MonoBehaviour
{
    public GameObject Zhib;
    // Start is called before the first frame update
    public void SendKill_(int x, int z)
    {
        StartCoroutine(UploadController.instance.SendKill(x, z));

    }

    void OnEnable()
    {
        UploadController.instance.OnDeath += SendKill_;
    }

    void OnDisable()
    {
        UploadController.instance.OnDeath -= SendKill_;
    }
}
