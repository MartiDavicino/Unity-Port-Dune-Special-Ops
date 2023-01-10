using UnityEngine;
using System.Collections;


public class HeatMapDrawer : MonoBehaviour
{
    [Range(1, 10)]
    public int gridDensity;

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
        float gridIncrement = (1f / gridDensity)*10;
        scale = gridIncrement*0.9f;

        scaleMultiplier =new Vector3 (scale, scale, scale);

        for (float i = 0; i < gridSize.x; i+=gridIncrement)
        {
            for(float j = 0; j < gridSize.y; j+=gridIncrement)
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

            indicator.transform.SetParent(transform);


            if (drawByHeight)
            {
                scaleMultiplier = new Vector3(scale, scale * value/1, scale);
                indicator.transform.localScale = scaleMultiplier;

                //offset object so is not displayed below the map
                Vector3 offset=new Vector3(0,0,0);
                indicator.transform.position += offset;
            }

            if (drawByWidth)
            {
                float widthMultiplier =value/10;
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

            //planes are bigger than other primitives
            if (_primitiveType == PrimitiveType.Plane)
            {
                indicator.transform.localScale = scaleMultiplier / 10;
            }
            else
                indicator.transform.localScale = scaleMultiplier;
        }
    }
}
