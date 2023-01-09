using UnityEngine;
using System.Collections;


public class HeatMapDrawer : MonoBehaviour
{

    float scale;
    Vector3 scaleMultiplier;
    Gradient colorGradient;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;


    // Start is called before the first frame update
    void Start()
    {
        int gridSize = 30;

        scale = 10f;
        scaleMultiplier =new Vector3 (scale, scale, scale);

        colorGradient = new Gradient ();

        colorKey = new GradientColorKey[2];
        colorKey[0].color = Color.green;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.red;
        colorKey[1].time = 1.0f;

        alphaKey = new GradientAlphaKey[2];

        alphaKey[0].alpha = 0.2f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 0.8f;
        colorGradient.SetKeys(colorKey, alphaKey);


        for (int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {
                float value = Random.Range(1, 10);
                DrawCube(i * 10, j * 10,value);
                //Debug.Log("Cube drawn");
                    
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawCube(float x,float y,float value)
    {
        scaleMultiplier = new Vector3(scale, scale*value, scale);
       
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "Heat: "+value;

        cube.transform.position = new Vector3(x,0,y);
        cube.transform.localScale = scaleMultiplier;


        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.color = colorGradient.Evaluate(value/10f);
        cube.GetComponent<Renderer>().material = newMaterial;
        
        //newMaterial.color=colorGradient.Evaluate(value*10f);

        //cube.GetComponent<Renderer>().material = newMaterial;
        
    }
}
