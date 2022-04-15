using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    HARKONNEN,
    SARDAUKAR,
    MENTAT,
    NONE,
}

public class EnemyBehaviour : MonoBehaviour
{
    enum state
    {
        SEEK, 
        PATROL
    }

    state currentState = state.SEEK;
    int speed = 20;
    public GameObject player;
    Transform lastPlayerPos;
    public int minRetargetingDistance = 3;
    public int minSeekDistance = 100;

    List<Vector3> oldWaypoint = new List<Vector3>();
    List<Vector3> waypoints = new List<Vector3>();
    List<Vector3> finalPath = new List<Vector3>();
    int currentPathIndex = 1;

     


    //public EnemyType type = EnemyType.NONE;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
