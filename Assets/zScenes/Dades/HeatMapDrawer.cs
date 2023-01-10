using UnityEngine;
using System.Collections;


public class HeatMapDrawer : MonoBehaviour
{
    [Range(1f, 10f)]
    public float gridDensity;

    float scale;
    Vector3 scaleMultiplier;
    Gradient colorGradient;
    public Gradient customGradient;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;
    public Color defaultColor;

    public PrimitiveType primitiveType;
    public bool drawByHeight,drawByWidth,drawByColor;
    [Range(0f, 1f)]
    public float thresholdDraw;

    // Start is called before the first frame update
    void Start()
    {
        //gridsize
        //330,133
        //-13,-310
        Vector2 initialPos = new Vector2(330, 133);
        Vector2 gridSize = new Vector2(330 - (-13), 133 - (-310));

        //If the desity increases the scale of the primitive should decrease

        scale = 1 / gridDensity;
        scaleMultiplier =new Vector3 (scale, scale, scale);

        for (int i = 0; i < gridSize.x; i++)
        {
            for(int j = 0; j < gridSize.y; j++)
            {
                float value = Random.Range(1, 10);
                DrawCube(initialPos.x-i,initialPos.y-j,value,drawByHeight,drawByWidth,drawByColor,thresholdDraw,primitiveType);
                    
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawCube(float x,float y,float value,bool _drawByHeight,bool _drawbyWidth,bool _drawByColor,float _thresholdDraw, PrimitiveType _primitiveType)
    {
        if (value > _thresholdDraw * 10)
        {
            scaleMultiplier = new Vector3(scale, scale, scale);


            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Plane);
            GameObject indicator = GameObject.CreatePrimitive(_primitiveType);
            indicator.name = "Heat: " + value;

            indicator.transform.position = new Vector3(x, 0.2f, y);


            if (drawByHeight)
            {
                scaleMultiplier = new Vector3(scale, scale * value, scale);
                indicator.transform.localScale = scaleMultiplier;

            }
            if (drawByWidth)
            {
                float widthMultiplier = value * 0.05f;
                scaleMultiplier = new Vector3(scale * widthMultiplier, scale, scale * widthMultiplier);
                indicator.transform.localScale = scaleMultiplier;
            }

            if (drawByColor)
            {
                Material newMaterial = new Material(Shader.Find("Standard"));
                newMaterial.color = customGradient.Evaluate(value / 10f);
                indicator.GetComponent<Renderer>().material = newMaterial;
            }
            else
                indicator.GetComponent<Renderer>().material.color = defaultColor;


        }

        
        
     
        
    }
}
