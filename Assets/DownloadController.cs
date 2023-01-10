using System.Data;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Networking;

public class IdList
{
    public int[] idList;
}
public class MovementData
{
    public int PosID = 0;
    public float x = 0f;
    public float z = 0f;
    public MovementData(int _id, float _x, float _z)
    {
        PosID = _id;
        x = _x;
        z = _z;
    }
    
   
}
public class DownloadController : MonoBehaviour
{
    private string dbName = "URI=file:Positions.db";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetPositions());
    }

    IEnumerator GetPositions()
    {
        //get the data and put it in forms
        WWWForm form = new WWWForm();
        form.AddField("Id", 1);
        form.AddField("x", 0);
        form.AddField("z", 0);
        
        using (UnityWebRequest www = UnityWebRequest.Get("https://citmalumnes.upc.es/~aitoram1/Delivery3/GetPositions.php"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                //Debug.Log(www.downloadHandler.text);
                
                // Or retrieve results as binary data
                string jsonArray = www.downloadHandler.text;

                //Debug.Log(jsonArray);

                //create a IdList and fill it with json
                IdList idList = JsonUtility.FromJson<IdList>(jsonArray);

                //print all the data from the idList
                for (int i = 0; i < idList.idList.Length; i++)
                {
                    Debug.Log(idList.idList[i]);
                }
                
                OnPositionsDownloaded(jsonArray);
            }
        }
    }

    void OnPositionsDownloaded(string arrayData)
    {
        //print in map
    }
}
