using System.Data;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class PositionData
{
    public int PosID = 0;
    public float x = 0f;
    public float z = 0f;
    public PositionData(int _id, float _x, float _z)
    {
        PosID = _id;
        x = _x;
        z = _z;
    }


}

public class KillData
{
    public int killID = 0;
    public float x = 0f;
    public float z = 0f;
    public KillData(int _killID, float _x, float _z)
    {
        killID = _killID;
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

    }

    public void GetPositionsData()
    {
        //StartCoroutine(GetPositions());
        StartCoroutine(GetKills());
    }

    public void KillsOut(KillData _killData)
    {
        Debug.Log(_killData.killID);
        Debug.Log(_killData.x);
        Debug.Log(_killData.z);
    }

    public void PositionOut(PositionData _posData)
    {
        //Debug.Log(_posData.PosID);
        //Debug.Log(_posData.x);
        //Debug.Log(_posData.z);
    }

    IEnumerator GetPositions()
    {
        //get the data and put it in forms
        WWWForm form = new WWWForm();
        form.AddField("PosId", 0);


        using (UnityWebRequest www = UnityWebRequest.Post("https://citmalumnes.upc.es/~aitoram1/Delivery3/GetPositions.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                List<PositionData> positionData = new List<PositionData>();

                // Show results as text
                //Debug.Log(www.downloadHandler.text);

                // Or retrieve results as binary data
                string rawResponse = www.downloadHandler.text;

                //split the raw response with "*"
                string[] positions = rawResponse.Split('*');

                for (int i = 0; i < positions.Length; i++)
                {
                    if (positions[i] != "")
                    {
                        string[] positionInfo = positions[i].Split(",");
                        //correct all the format errors of positionInfo


                        for (int y = 0; y < positionInfo.Length; y++)
                        {
                            //check that positionInfo[y] is not empty or in the end of the string
                            if (positionInfo[y] != "" && positionInfo[y] != " ")
                            {
                                //check if the positionInfo[y] is a number
                                if (int.TryParse(positionInfo[y], out int n))
                                {
                                    //check if try parse was succesfull
                                    if (n != 0)
                                    {
                                        positionData.Add(new PositionData(int.Parse(positionInfo[0]), int.Parse(positionInfo[1]), int.Parse(positionInfo[2])));
                                        PositionOut(positionData[y]);
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }
    }
    IEnumerator GetKills()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://citmalumnes.upc.es/~aitoram1/Delivery3/GetKills.php"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                List<KillData> KillList = new List<KillData>();

                // Show results as text
                //Debug.Log(www.downloadHandler.text);

                // Or retrieve results as binary data
                string rawResponse = www.downloadHandler.text;

                //split the raw response with "*"
                string[] kills = rawResponse.Split('*');

                for (int i = 0; i < kills.Length; i++)
                {
                    if (kills[i] != "")
                    {
                        string[] killsInfo = kills[i].Split(",");
                        //correct all the format errors of positionInfo

                        for (int y = 0; y < killsInfo.Length; y++)
                        {
                            //check that positionInfo[y] is not empty or in the end of the string
                            if (killsInfo[y] != "" && killsInfo[y] != " ")
                            {
                                //check if the positionInfo[y] is a number
                                if (int.TryParse(killsInfo[y], out int n))
                                {
                                    //check if try parse was succesfull
                                    if (n != 0)
                                    {
                                        KillList.Add(new KillData(int.Parse(killsInfo[0]), int.Parse(killsInfo[1]), int.Parse(killsInfo[2])));
                                        KillsOut(KillList[y]);
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }
    }
    IEnumerator GetDeaths()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://citmalumnes.upc.es/~aitoram1/Delivery3/GetDeaths.php"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                List<KillData> KillList = new List<KillData>();

                // Show results as text
                //Debug.Log(www.downloadHandler.text);

                // Or retrieve results as binary data
                string rawResponse = www.downloadHandler.text;

                //split the raw response with "*"
                string[] kills = rawResponse.Split('*');

                for (int i = 0; i < kills.Length; i++)
                {
                    if (kills[i] != "")
                    {
                        string[] killsInfo = kills[i].Split(",");
                        //correct all the format errors of positionInfo

                        for (int y = 0; y < killsInfo.Length; y++)
                        {
                            //check that positionInfo[y] is not empty or in the end of the string
                            if (killsInfo[y] != "" && killsInfo[y] != " ")
                            {
                                //check if the positionInfo[y] is a number
                                if (int.TryParse(killsInfo[y], out int n))
                                {
                                    //check if try parse was succesfull
                                    if (n != 0)
                                    {
                                        KillList.Add(new KillData(int.Parse(killsInfo[0]), int.Parse(killsInfo[1]), int.Parse(killsInfo[2])));
                                        KillsOut(KillList[y]);
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }
    }
}















