using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendDeath : MonoBehaviour
{

    public GameObject Zhib;
    public UploadController uploader;
    // Start is called before the first frame update
    public void Send(int x,int z)
    {
        StartCoroutine(uploader.SendDeath((int)Zhib.transform.position.x, (int)Zhib.transform.position.y));

    }

    void OnDisable()
    {
        UploadController.OnDeath -= Send;
    }

    public void Position(int x, int z)
    {

    }
}
