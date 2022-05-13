using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Cliff : MonoBehaviour
{
    public bool oneWay;
    public enum direction
    {
        UP_TO_DOWN,
        DOWN_TO_UP,
        NONE
    }

    public direction oneWayDirection;

    public LayerMask whatIsPlayer;

    private bool isUp;
    private CharacterBaseBehavior baseScript;
    private NavMeshAgent playerAgent;

    private bool canActivate;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] characters = Physics.OverlapSphere(transform.position, 5.0f, whatIsPlayer);

        for (int i = 0; i < characters.Length; i++)
        {
            canActivate = true;

            baseScript = characters[i].gameObject.GetComponent<CharacterBaseBehavior>();
            playerAgent = characters[i].gameObject.GetComponent<NavMeshAgent>();

            float toleranceRange = 1f;
            Transform upPoint = transform.Find("upPoint");
            Transform downPoint = transform.Find("downPoint");

            if (characters[i].transform.position.y - toleranceRange < downPoint.position.y && characters[i].transform.position.y + toleranceRange < upPoint.position.y)
                isUp = false;
            else if (characters[i].transform.position.y - toleranceRange > downPoint.position.y && characters[i].transform.position.y + toleranceRange > upPoint.position.y)
                isUp = true;

            if (Input.GetKeyDown(KeyCode.F))
            {

                if(oneWay)
                {
                    switch(oneWayDirection)
                    {
                        case direction.UP_TO_DOWN:
                            if (isUp)
                            {
                                baseScript.state = PlayerState.IDLE;
                                playerAgent.Warp(downPoint.position);
                                playerAgent.ResetPath();
                            }
                            break;

                        case direction.DOWN_TO_UP:
                            if (!isUp)
                            {
                                baseScript.state = PlayerState.IDLE;
                                playerAgent.Warp(upPoint.position);
                                playerAgent.ResetPath();
                            }
                            break;

                        case direction.NONE:
                            break;
                    }
                } else
                {
                    if (isUp)
                    {
                        baseScript.state = PlayerState.IDLE;
                        playerAgent.Warp(downPoint.position);
                        playerAgent.ResetPath();
                    }
                    else
                    {
                        baseScript.state = PlayerState.IDLE;
                        playerAgent.Warp(upPoint.position);
                        playerAgent.ResetPath();
                    }
                }

            }
            
        }

        if (characters.Length == 0)
        {
            canActivate = false;
        }
    }

    private void OnGUI()
    {
        if (canActivate)
        {

            if (oneWay)
            {
                switch (oneWayDirection)
                {
                    case direction.UP_TO_DOWN:
                        if (isUp)
                        {
                            GUI.Box(new Rect(Screen.width - 155, Screen.height - 45, 150, 40), "Press 'f' to\nclimb down");
                        }
                        break;

                    case direction.DOWN_TO_UP:
                        if (!isUp)
                        {
                            GUI.Box(new Rect(Screen.width - 155, Screen.height - 45, 150, 40), "Press 'f' to\nclimb up");
                        }
                        break;

                    case direction.NONE:
                        break;
                }
            }
            else
            {
                if (isUp)
                {
                    GUI.Box(new Rect(Screen.width - 155, Screen.height - 45, 150, 40), "Press 'f' to\nclimb down");

                }
                else
                {
                    GUI.Box(new Rect(Screen.width - 155, Screen.height - 45, 150, 40), "Press 'f' to\nclimb up");

                }
            }

        }
    }
}
