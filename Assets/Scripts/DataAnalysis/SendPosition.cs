using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPosition : MonoBehaviour
{
    public GameObject Zhib;
    public UploadController uploader;

    public float updateInterval = 1.0F;
    private double lastInterval;
    private int frames;
    private float fps;
    // Start is called before the first frame update
    void OnDisable()
    {
        UploadController.OnMovement -= SendPos_;
    }
    void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }

    void SendPos_(int x, int z)
    {
        StartCoroutine(uploader.SendPos(x, z));
    }

    // Update is called once per frame
    void Update()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;

            UploadController.OnMovement += SendPos_;
        }
    }
}
