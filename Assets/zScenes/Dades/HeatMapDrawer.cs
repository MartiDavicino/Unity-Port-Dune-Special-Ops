using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System.Xml.Linq;

public class HeatMapDrawer : MonoBehaviour
{
    public Material defaultMaterial;

    Vector2 initialPos;
    [Range(1, 10)]
    public int gridDensity;
    [HideInInspector]
    public Vector2 gridSize;

    float scale;
    Vector3 scaleMultiplier;
    float gridIncrement;
    Gradient colorGradient;
    public Gradient heatGradient;
    public Gradient contrastGradient;
    public Gradient constantGradient;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;
    public Color defaultColor;

    public PrimitiveType primitiveType;
    public bool drawByHeight, drawByWidth, drawByColor, drawByTransparency;
    [Range(0f, 1f)]
    public float thresholdDraw;

    //DATA
    private static DownloadController downloadInstance;

    private List<GameObject> cubes = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //gridsize
        //330,133
        //-13,-310
        initialPos = new Vector2(330, 133);
        gridSize = new Vector2(330 - (-13), 133 - (-310));

        //scale
        gridIncrement = (1f / gridDensity) * 10;
        scale = gridIncrement * 0.9f;
        scaleMultiplier = new Vector3(scale, scale, scale);

        //DrawGrid(gridSize);

        downloadInstance = DownloadController.MyDownloadInstance;
        DrawPositionsFromData();

    }


    // Update is called once per frame
    void Update()
    {

    }

    public void ClearMap()
    {
        foreach (GameObject el in cubes)
        {
            Destroy(el);
        }
    }


    public void GenerateMap()
    {
        ClearMap();

        if (downloadInstance.positionList.Count != 0)
        {
            DrawPositionsFromData();
        }
        else if (downloadInstance.killList.Count != 0)
        {
            DrawKillsFromData();
        }
        else if (downloadInstance.deathList.Count != 0)
        {
            DrawDeathsFromData();
        }
    }

    public void DrawPositionsFromData()
    {
        if (downloadInstance.positionList.Count > 0)
        {
            Debug.Log("Draw from controller from list with " + downloadInstance.positionList.Count + " positions");


            int maxTimes = 10;


            for (int i = 0; i < downloadInstance.positionList.Count; i++)
            {
                Debug.Log("Max times" + downloadInstance.positionList[i].PosID);

                if (downloadInstance.positionList[0].PosID > maxTimes)
                {
                    maxTimes = downloadInstance.positionList[0].PosID;
                }

            }



            //Add all the visited times to each tile

            foreach (PositionData element in downloadInstance.positionList)
            {
                //DrawSimpleCube(element.x, element.z, element.PosID);
                int value = 0;
                if (element.PosID > 10)
                {
                    value = 10;
                }
                else
                    value = element.PosID;

                float normalized = (float)value / (float)maxTimes; 
                Debug.Log("norm"+normalized);


                DrawCube(element.x, element.z, normalized*10.0f, drawByHeight, drawByWidth, drawByColor, drawByTransparency, thresholdDraw, primitiveType);

               
            }
            for (int i = 0; i < 1000; i++)
            {
                int index = Random.Range(0, downloadInstance.positionList.Count());

                DrawSimpleCube(downloadInstance.positionList[index].x, downloadInstance.positionList[index].z, downloadInstance.positionList[index].PosID);

            }
       
        }
        else
            Debug.Log("Positions list still not loaded");

    }

    public void DrawKillsFromData()
    {
        if (downloadInstance.killList.Count > 0)
        {
            Debug.Log("Draw from controller from list with " + downloadInstance.killList.Count + " positions");

            int maxTimes = 0;
            //Look for max

            for (int i = 0; i < downloadInstance.killList.Count; i++)
            {
                Debug.Log("Max times" + downloadInstance.killList[i].killID);

                if (downloadInstance.killList[0].killID > maxTimes)
                {
                    maxTimes = downloadInstance.killList[0].killID;
                }

            }


            //Add all the visited times to each tile

            foreach (KillData element in downloadInstance.killList)
            {
                //DrawSimpleCube(element.x, element.z, element.PosID);

                DrawCube(element.x, element.z, element.killID, drawByHeight, drawByWidth, drawByColor, drawByTransparency, thresholdDraw, primitiveType);

                //foreach (PositionData element in downloadInstance.positionList)
                //{


                //    int index = Random.Range(0, downloadInstance.positionList.Count());

                //    DrawSimpleCube(element.x, element.z, element.PosID);

                //}
            }
            for (int i = 0; i < 1000; i++)
            {
                int index = Random.Range(0, downloadInstance.positionList.Count());

                DrawSimpleCube(downloadInstance.killList[index].x, downloadInstance.killList[index].z, downloadInstance.killList[index].killID);

            }
            // for (int i = 0; i < downloadInstance.positionList.Count; i++)
            // {
            //     Debug.Log("Drawing pos");
            //     DrawSimpleCube(downloadInstance.positionList.ElementAt(i).x, downloadInstance.positionList.ElementAt(i).x, downloadInstance.positionList.ElementAt(i).PosID);
            // }
        }
        else
            Debug.Log("Positions list still not loaded");

    }

    public void DrawDeathsFromData()
    {
        if (downloadInstance.deathList.Count > 0)
        {
            Debug.Log("Draw from controller from list with " + downloadInstance.deathList.Count + " positions");

            int maxTimes = 0;
            //Look for max

            for (int i = 0; i < downloadInstance.deathList.Count; i++)
            {
                Debug.Log("Max times" + downloadInstance.deathList[i].deathID);

                if (downloadInstance.deathList[0].deathID > maxTimes)
                {
                    maxTimes = downloadInstance.deathList[0].deathID;
                }

            }


            //Add all the visited times to each tile

            foreach (DeathsData element in downloadInstance.deathList)
            {
                //DrawSimpleCube(element.x, element.z, element.PosID);

                DrawCube(element.x, element.z, element.deathID, drawByHeight, drawByWidth, drawByColor, drawByTransparency, thresholdDraw, primitiveType);

                //foreach (PositionData element in downloadInstance.positionList)
                //{


                //    int index = Random.Range(0, downloadInstance.positionList.Count());

                //    DrawSimpleCube(element.x, element.z, element.PosID);

                //}
            }
            for (int i = 0; i < 1000; i++)
            {
                int index = Random.Range(0, downloadInstance.deathList.Count());

                DrawSimpleCube(downloadInstance.deathList[index].x, downloadInstance.deathList[index].z, downloadInstance.deathList[index].deathID);

            }
            // for (int i = 0; i < downloadInstance.positionList.Count; i++)
            // {
            //     Debug.Log("Drawing pos");
            //     DrawSimpleCube(downloadInstance.positionList.ElementAt(i).x, downloadInstance.positionList.ElementAt(i).x, downloadInstance.positionList.ElementAt(i).PosID);
            // }
        }
        else
            Debug.Log("Positions list still not loaded");

    }


    void DrawWithoutGrid()
    {

    }

    void DrawPath()
    {

    }

    void DrawGrid(Vector2 _gridSize)
    {


        for (float i = 0; i < _gridSize.x; i += gridIncrement)
        {
            for (float j = 0; j < _gridSize.y; j += gridIncrement)
            {
                float value = Random.Range(1, 10);
                DrawCube(initialPos.x - i, initialPos.y - j, value, drawByHeight, drawByWidth, drawByColor, drawByTransparency, thresholdDraw, primitiveType);

            }
        }
    }


    void DrawCube(float x, float y, float value, bool _drawByHeight, bool _drawbyWidth, bool _drawByColor, bool _drawByTransparency, float _thresholdDraw, PrimitiveType _primitiveType)
    {
        if (value > _thresholdDraw * 10)
        {
            scaleMultiplier = new Vector3(scale, scale, scale);


            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Plane);
            GameObject indicator = GameObject.CreatePrimitive(_primitiveType);
            indicator.name = "Heat: " + value;

            indicator.transform.position = new Vector3(x, 0.0f, y);

            indicator.transform.SetParent(transform);


            if (_drawByHeight)
            {
                scaleMultiplier = new Vector3(scale, scale * value * 0.1f, scale);
                indicator.transform.localScale = scaleMultiplier;

                //offset object so is not displayed below the map
                Vector3 offset = new Vector3(1, 20, 1);
                indicator.transform.position = Vector3.Scale(indicator.transform.position, offset);
            }

            if (_drawbyWidth)
            {
                float widthMultiplier = value / 10;
                scaleMultiplier = new Vector3(scale * widthMultiplier, scale, scale * widthMultiplier);
                indicator.transform.localScale = scaleMultiplier;
            }

            indicator.GetComponent<Renderer>().material = defaultMaterial;
            if (_drawByColor)
            {
                indicator.GetComponent<Renderer>().material.color = constantGradient.Evaluate(value / 10f);
            }
            else
            {
                indicator.GetComponent<Renderer>().material.color = defaultColor;
            }

            if (_drawByTransparency)
            {
                Color transparentColor = indicator.GetComponent<Renderer>().material.color;
                transparentColor.a = value / 10f;
                indicator.GetComponent<Renderer>().material.color = transparentColor;
            }


            //material transprency


            //planes are bigger than other primitives
            if (_primitiveType == PrimitiveType.Plane)
            {
                indicator.transform.localScale = scaleMultiplier / 10;
            }
            else
                indicator.transform.localScale = scaleMultiplier;

            cubes.Add(indicator);
        }
    }

    void DrawSimpleCube(float _x, float _z, int times)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(_x, 0.0f, _z);
        cube.transform.SetParent(transform);
        cube.transform.localScale = new Vector3(1, times, 1);
        cubes.Add(cube);

    }


}