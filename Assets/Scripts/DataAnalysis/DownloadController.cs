using System.Data;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using System;

public class PositionData
{
    public int PosID = 0;
    public int x = 0;
    public int z = 0;
    public PositionData(int _id, int _x, int _z)
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

public class DeathsData
{
    public int deathID = 0;
    public float x = 0f;
    public float z = 0f;
    public DeathsData(int _deathID, float _x, float _z)
    {
        deathID = _deathID;
        x = _x;
        z = _z;
    }
}

public class DownloadController : MonoBehaviour
{



    public List<PositionData> positionList = new List<PositionData>();
    public List<KillData> killList = new List<KillData>();
    public List<DeathsData> deathList = new List<DeathsData>();

    private static DownloadController downloadInstance;

    public static DownloadController MyDownloadInstance
    {
        get
        {
            if (downloadInstance == null)
            {
                downloadInstance = FindObjectOfType<DownloadController>();
            }
            return downloadInstance;
        }


    }
    // Start is called before the first frame update
    void Start()
    {
    }

    public void ClearLists(){
        positionList.Clear();
        killList.Clear();
        deathList.Clear();
    }

    public void GetPositionsData()
    {
        ClearLists();
        StartCoroutine(GetPositions());
    }

    public void GetKillsData()
    {
        ClearLists();
        StartCoroutine(GetKills());
    }

    public void GetDeathsData()
    {
        ClearLists();
        StartCoroutine(GetDeaths());
    }

    public void PositionOut(PositionData _posData)
    {
        // Debug.Log(_posData.PosID);
        // Debug.Log(_posData.x);
        // Debug.Log(_posData.z);

        if (!positionList.Contains(_posData))
            positionList.Add(_posData);
    }
    public void KillsOut(KillData _killData)
    {
        Debug.Log(_killData.killID);
        Debug.Log(_killData.x);
        Debug.Log(_killData.z);

        if (!killList.Contains(_killData))
            killList.Add(_killData);
    }
    public void DeathsOut(DeathsData _deathsData)
    {
        Debug.Log(_deathsData.deathID);
        Debug.Log(_deathsData.x);
        Debug.Log(_deathsData.z);
    }

    int RoundBy(float number, int r)
    {
        int myNumber = (int)number;
        myNumber = myNumber / r;
        myNumber = myNumber * r;

        return myNumber;
    }

    IEnumerator GetPositions()
    {
        //get the data and put it in forms
        WWWForm form = new WWWForm();
        form.AddField("PosId", 0);


        using (UnityWebRequest www = UnityWebRequest.Post("https://citmalumnes.upc.es/~aitoram1/Delivery3/GetPositions.php", form))
        {
            yield return www.SendWebRequest();
            Debug.Log("starting download...");
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {

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
                                        int count = int.Parse(positionInfo[0]);
                                        int x = int.Parse(positionInfo[1]);
                                        int z = int.Parse(positionInfo[2]);

                                        //TODO: Avoid Duplicates!
                                        if (!positionList.Contains(new PositionData(count, x, z)))
                                        {
                                            Debug.Log("COunt" + count.ToString());
                                            positionList.Add(new PositionData(count, x, z));
                                            // PositionOut(positionList[y]);
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
                                        int count = int.Parse(killsInfo[0]);
                                        int x = int.Parse(killsInfo[1]);
                                        int z = int.Parse(killsInfo[2]);
                                        killList.Add(new KillData(count, x, z));
                                        // KillsOut(killList[y]);
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
                // Show results as text
                //Debug.Log(www.downloadHandler.text);

                // Or retrieve results as binary data
                string rawResponse = www.downloadHandler.text;
                Debug.Log(rawResponse);

                //split the raw response with "*"
                string[] deaths = rawResponse.Split('*');

                for (int i = 0; i < deaths.Length; i++)
                {
                    if (deaths[i] != "")
                    {
                        string[] deathsInfo = deaths[i].Split(",");
                        //correct all the format errors of positionInfo

                        for (int y = 0; y < deathsInfo.Length; y++)
                        {
                            //check that positionInfo[y] is not empty or in the end of the string
                            if (deathsInfo[y] != "" && deathsInfo[y] != " ")
                            {
                                //check if the positionInfo[y] is a number
                                if (int.TryParse(deathsInfo[y], out int n))
                                {
                                    //check if try parse was succesfull
                                    if (n != 0)
                                    {
                                        int count = int.Parse(deathsInfo[0]);
                                        int x = int.Parse(deathsInfo[1]);
                                        int z = int.Parse(deathsInfo[2]);
                                        deathList.Add(new DeathsData(count, x, z));
                                        //  DeathsOut(DeathsList[y]);
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














