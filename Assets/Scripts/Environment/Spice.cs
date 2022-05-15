using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spice : MonoBehaviour
{

    public int spiceAmmount;
    public LayerMask whatIsPlayer;

    private Camera playerCamera;

    public Mesh smallSack;
    public Mesh mediumSack;
    public Mesh bigSack;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        Mesh myMesh = new Mesh();

        if (spiceAmmount > 140)
        {
            myMesh = bigSack;
        }
        else if (spiceAmmount <= 140 && spiceAmmount > 70)
        {
            myMesh = mediumSack;
        }
        else if (spiceAmmount <= 70)
        {
            myMesh = smallSack;
        }

        
        gameObject.transform.rotation = Quaternion.Euler(-90, 0, 0);
        

        GetComponent<MeshFilter>().mesh = myMesh;

        Collider[] players = Physics.OverlapSphere(transform.position, 3.0f, whatIsPlayer);
        if (players.Length > 0)
        {
            GeneralManager manager = playerCamera.GetComponent<GeneralManager>();
            manager.totalSpice += spiceAmmount;
            Destroy(gameObject);
        }
    }
}
