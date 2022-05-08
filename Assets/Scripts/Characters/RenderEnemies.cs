using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderEnemies : MonoBehaviour
{
    public float renderRadius;
    public LayerMask whatIsEnemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, renderRadius, whatIsEnemy);

        for (int i = 0; i < enemies.Length; i++)
        {
            string deleteWord = "(Clone)";
            
            Transform child = enemies[i].gameObject.transform.Find(enemies[i].name.Replace(deleteWord, "") + "_low");
            child.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }
    }
}
