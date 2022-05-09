using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderEnemies : MonoBehaviour
{
    public float renderRadius;
    public LayerMask whatIsEnemy;
    private bool debug;
    private bool once;

    // Start is called before the first frame update
    void Start()
    {
        once = true;
        debug = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            once = true;
            debug = !debug;
        }

        if(debug)
        {
            if(once)
            {
                gameObject.AddComponent<LineRenderer>();
                once = false;
            }

            gameObject.DrawCircleScaled(renderRadius, 0.05f, transform.localScale);

        } else
        {
            if(once)
            {
                Destroy(gameObject.GetComponent<LineRenderer>());
                once = false;
            }
        }

    }

    // Update is called once per frame
    void LateUpdate()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, renderRadius, whatIsEnemy);

        for (int i = 0; i < enemies.Length; i++)
        {
            string[] splitArray = enemies[i].name.Split(char.Parse(" "));
            string[] splitArray2 = splitArray[0].Split(char.Parse("("));

            string finalName = splitArray2[0];

            Transform child = enemies[i].gameObject.transform.Find(finalName + "_low");

            child.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }
    }
}
