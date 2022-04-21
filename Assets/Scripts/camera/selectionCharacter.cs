using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectionCharacter : MonoBehaviour
{
    
    private CameraMovement cameraScript;
    private GameObject selectedCharacter;

    // Start is called before the first frame update
    void Start()
    {
        cameraScript = gameObject.GetComponent<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Z))
        {
            GameObject temp = GameObject.Find("Zhib");

            cameraScript.focusedPlayer = temp;

            characterwalkingscript walkScript = temp.GetComponent<characterwalkingscript>();
            walkScript.selectedCharacter = true;


            temp = GameObject.Find("Nerala");
            walkScript = temp.GetComponent<characterwalkingscript>();
            walkScript.selectedCharacter = false;
        }

        if (Input.GetKey(KeyCode.X))
        {
            GameObject temp = GameObject.Find("Nerala");

            cameraScript.focusedPlayer = temp;

            characterwalkingscript walkScript = temp.GetComponent<characterwalkingscript>();
            walkScript.selectedCharacter = true;

            temp = GameObject.Find("Zhib");
            walkScript = temp.GetComponent<characterwalkingscript>();
            walkScript.selectedCharacter = false;
        }

        //if (Input.GetKey(KeyCode.Z))
        //cameraScript.focusedPlayer = GameObject.Find("Zhib");
    }
}
