using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
public class UploadController : MonoBehaviour
{
    // Start is called before the first frame update
    //Upload Player
    private static UploadController uploadController;

    public SendDeath senderOfDeath;
    public static UploadController instance
    {
        get
        {
            if (uploadController == null)
            {
                uploadController = FindObjectOfType<UploadController>();
            }
            return uploadController;
        }


    }

    public Action<int, int> OnMovement; //Movement, Kill and Death
    public Action<int, int> OnKill;
    public Action<int, int> OnDeath;

    private void OnEnable()
    {
    }
    public IEnumerator SendPos(int _x, int _z)
    {
        yield return new WaitForEndOfFrame();

        //Debug.Log("Sending Position...");

        int x = _x;
        int z = _z;

        if (x != 7 && z != 101 || x != 262 && z != 44)
        {
            WWWForm form = new WWWForm();
            form.AddField("x", x);
            form.AddField("z", z);

            using (var request = UnityWebRequest.Post("https://citmalumnes.upc.es/~aitoram1/Delivery3/SendPosition.php", form))
            {
                request.downloadHandler = new DownloadHandlerBuffer();

                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(request.error);
                }
            }
        }

        
    }

    //UploadSession
    public IEnumerator SendKill(int _x, int _z)
    {
        yield return new WaitForEndOfFrame();

        Debug.Log("Sending Kill...");


        int x = _x;
        int z = _z;

        WWWForm form = new WWWForm();
        form.AddField("x", x);
        form.AddField("z", z);

        using (var request = UnityWebRequest.Post("https://citmalumnes.upc.es/~aitoram1/Delivery3/SendKill.php", form))
        {
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }

        }
    }

    public IEnumerator SendDeath(int _x, int _z)
    {

        Debug.Log("Sending Death...");

        int x = _x;
        int z = _z;

        WWWForm form = new WWWForm();
        form.AddField("x", x);
        form.AddField("z", z);

        using (var request = UnityWebRequest.Post("https://citmalumnes.upc.es/~aitoram1/Delivery3/SendDeath.php", form))
        {
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }

        }
        OnDeath -= senderOfDeath.SendDeath_;

    }
}

public class CallbackEvents
{
    public static Action<int, int> OnMovementCallback;
    public static Action<int, int> OnKillCallback;
    public static Action<int, int> OnDearthCallback;
}
