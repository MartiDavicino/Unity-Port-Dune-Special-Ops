using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GeneralManager : MonoBehaviour
{
    public GameObject zhib;
    private CharacterBaseBehavior zhibBase;

    public GameObject nerala;
    private CharacterBaseBehavior neralaBase;

    private GameObject hunterSeeker;
    private HunterSeekerAbility hunterSeekerBase;
    private bool hunterSeekerActive;

    private GameObject selectedCharacter;
    private bool allSelected;

    private CameraMovement cameraScript;

    public bool gameLost;

    // Start is called before the first frame update
    void Start()
    {
        zhibBase = zhib.GetComponent<CharacterBaseBehavior>();

        neralaBase = nerala.GetComponent<CharacterBaseBehavior>();
        hunterSeekerBase = nerala.GetComponent<HunterSeekerAbility>();
        hunterSeekerActive = false;

        cameraScript = gameObject.GetComponent<CameraMovement>();

        selectedCharacter = zhib;

        gameLost = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameLost)
        {
            if (zhibBase.playerHealth == 0)
            {
                Destroy(zhib);
                selectedCharacter = nerala;
            }

            if (neralaBase.playerHealth == 0)
            {
                Destroy(nerala);
                selectedCharacter = zhib;
            }

            if (zhibBase.playerHealth == 0 && neralaBase.playerHealth == 0)
                gameLost = true;

            if (Input.GetKey(KeyCode.Z))
            {
                selectedCharacter = zhib;
                allSelected = false;
            }

            if (Input.GetKey(KeyCode.X) && !hunterSeekerActive)
            {
                selectedCharacter = nerala;
                allSelected = false;
            }

            if (Input.GetKey(KeyCode.C) && !hunterSeekerActive)
            {
                allSelected = true;
                zhibBase.allSelected = true;
                neralaBase.allSelected = true;
            }


            if (hunterSeekerActive && selectedCharacter == null)
            {
                selectedCharacter = nerala;
                hunterSeekerActive = false;
            }


            if(selectedCharacter == nerala)
            {
                if(GameObject.Find("HunterSeeker(Clone)") != null)
                {
                    hunterSeeker = GameObject.Find("HunterSeeker(Clone)");
                    selectedCharacter = hunterSeeker;
                    hunterSeekerActive = true;
                }
            }


            if(!allSelected)
            {
                switch (selectedCharacter.name)
                {
                    case "Zhib":
                        zhibBase.selectedCharacter = true;
                        zhibBase.allSelected = false;

                        neralaBase.selectedCharacter = false;
                        neralaBase.allSelected = false;
                        break;

                    case "Nerala":
                        neralaBase.selectedCharacter = true;
                        neralaBase.allSelected = false;

                        zhibBase.selectedCharacter = false;
                        zhibBase.allSelected = false;
                        break;

                    case "HunterSeeker(Clone)":
                        neralaBase.selectedCharacter = false;
                        neralaBase.allSelected = false;

                        zhibBase.selectedCharacter = false;
                        zhibBase.allSelected = false;
                        break;
                }
            }

            cameraScript.focusedPlayer = selectedCharacter;
        } else
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        
    }

    void OnGUI()
    {
        if(gameLost)
            GUI.Box(new Rect(Screen.width/2 - 125, Screen.height/2, 250, 30), "Press 'R' to restart");
    }
}
