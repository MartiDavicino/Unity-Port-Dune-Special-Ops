using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectionCharacter : MonoBehaviour
{
    
    private CameraMovement cameraScript;

    private HunterSeekerAbility hunterSeekerScript;

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

            CharacterBaseBehavior baseScript = temp.GetComponent<CharacterBaseBehavior>();
            baseScript.selectedCharacter = true;


            temp = GameObject.Find("Nerala");
            baseScript = temp.GetComponent<CharacterBaseBehavior>();
            baseScript.selectedCharacter = false;
        }

        if (Input.GetKey(KeyCode.X))
        {
            GameObject temp = GameObject.Find("Nerala");

            hunterSeekerScript = temp.GetComponent<HunterSeekerAbility>();

            cameraScript.focusedPlayer = temp;

            CharacterBaseBehavior baseScript = temp.GetComponent<CharacterBaseBehavior>();
            baseScript.selectedCharacter = true;

            temp = GameObject.Find("Zhib");
            baseScript = temp.GetComponent<CharacterBaseBehavior>();
            baseScript.selectedCharacter = false;
        }

        if(cameraScript.focusedPlayer == GameObject.Find("Nerala") && hunterSeekerScript.seekerHunting)
        {
            GameObject temp = GameObject.Find("HunterSeeker(Clone)");
            cameraScript.focusedPlayer = temp;

            temp = GameObject.Find("Nerala");
            CharacterBaseBehavior baseScript = temp.GetComponent<CharacterBaseBehavior>();
            baseScript.selectedCharacter = false;
        }
    }
}
