using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    Gradient colorGradient;
    public Gradient heatGradient;
    public Gradient contrastGradient;
    public Gradient constantGradient;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;
    public Color defaultColor;

    public PrimitiveType primitiveType;
    public bool drawByHeight,drawByWidth,drawByColor,drawByTransparency;
    [Range(0f, 1f)]
    public float thresholdDraw;

    //DATA
    private static DownloadController downloadInstance;

    // Start is called before the first frame update
    void Start()
    {
        //gridsize
        //330,133
        //-13,-310
        initialPos = new Vector2(330, 133);
        gridSize = new Vector2(330 - (-13), 133 - (-310));

        //DrawGrid(gridSize);

        downloadInstance = DownloadController.MyDownloadInstance;
        DrawPositionsFromData();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public class PositionInfo
    {
        public int x;
        public int z;
        public int times = 1;
        public PositionInfo(int _x, int _z)
        {
            x = _x;
            z = _z;
        }

    }

    public void DrawPositionsFromData()
    {
        if (downloadInstance.positionList.Count > 0)
        {
            int round = 10;
            //bool[,] positionOcupied = new bool[(int)gridSize.x,(int)gridSize.y];
            //int[,] timesOcupied = new int[(int)gridSize.x, (int)gridSize.y];

            List<PositionInfo> posList = new List<PositionInfo>();

            Debug.Log("Draw from controller from list with "+downloadInstance.positionList.Count+" positions");

            //Add all the visited times to each tile
            for (int i = 0; i < downloadInstance.positionList.Count; i++)
            {
                //timesOcupied[RoundBy(downloadInstance.positionList[i].x, round), RoundBy(downloadInstance.positionList[i].z, round)]++;
                //Debug.Log("Pillao");


                int x = RoundBy(downloadInstance.positionList[i].x, round);
                int z = RoundBy(downloadInstance.positionList[i].z, round);

                PositionInfo temporal = new PositionInfo(x,z);


                if (posList.Contains(temporal))
                {
                    int index = posList.IndexOf(temporal);
                    posList[index].times += 1;
                }
                else
                {
                    posList.Add(temporal);
                }


            }


            for (int i = 0; i < posList.Count; i++)
            {
                Debug.Log("Drawing pos");
                DrawSimpleCube(posList[i].x, posList[i].z, posList[i].times);
            }
        }
        else
            Debug.Log("Positions list still not loaded");
        
    }
    int RoundBy(float number,int r)
    {
        int myNumber=(int)number;
        myNumber=myNumber / r;
        myNumber=myNumber * r;

        return myNumber;
    }

    void DrawWithoutGrid()
    {

    }

    void DrawPath()
    {

    }

    void DrawGrid(Vector2 _gridSize)
    {
        float gridIncrement = (1f / gridDensity) * 10;
        scale = gridIncrement * 0.9f;

        scaleMultiplier = new Vector3(scale, scale, scale);

        for (float i = 0; i < _gridSize.x; i += gridIncrement)
        {
            for (float j = 0; j < _gridSize.y; j += gridIncrement)
            {
                float value = Random.Range(1, 10);
                DrawCube(initialPos.x - i, initialPos.y - j, value, drawByHeight, drawByWidth, drawByColor,drawByTransparency, thresholdDraw, primitiveType);

            }
        }
    }


    void DrawCube(float x,float y,float value,bool _drawByHeight,bool _drawbyWidth,bool _drawByColor,bool _drawByTransparency,float _thresholdDraw, PrimitiveType _primitiveType)
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
                scaleMultiplier = new Vector3(scale, scale * value*0.1f, scale);
                indicator.transform.localScale = scaleMultiplier;

                //offset object so is not displayed below the map
                Vector3 offset=new Vector3(1,20,1);
                indicator.transform.position = Vector3.Scale(indicator.transform.position,offset);
            }

            if (_drawbyWidth)
            {
                float widthMultiplier =value/10;
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
                transparentColor.a = value/10f;
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
        }
    }

    void DrawSimpleCube(float _x,float _z,int times)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(_x, 0.0f, _z);
        cube.transform.SetParent(transform);
        cube.transform.localScale = new Vector3(1,times,1);

    }
}
