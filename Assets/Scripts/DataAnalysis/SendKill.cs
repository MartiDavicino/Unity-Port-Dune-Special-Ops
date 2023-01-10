using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendKill : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        UploadController.instance.OnKill += SendKill_;
    }

    void OnDisable()
    {
        UploadController.instance.OnKill -= SendKill_;
    }

    void SendKill_(int x, int z)
    {
        StartCoroutine(UploadController.instance.SendKill(x, z));

    }
}
