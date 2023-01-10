using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GeneralManager : MonoBehaviour
{
    public int totalSpice;

    public GameObject zhib;
    private CharacterBaseBehavior zhibBase;

    public GameObject nerala;
    private CharacterBaseBehavior neralaBase;
    public bool neralaUnlocked;

    private GameObject hunterSeeker;
    private HunterSeekerAbility hunterSeekerBase;
    private bool hunterSeekerActive;

    public GameObject omozra;
    private CharacterBaseBehavior omozraBase;
    public bool omozraUnlocked;

    public GameObject sadiq;

    private GameObject selectedCharacter;
    private bool allSelected;

    private CameraMovement cameraScript;

    public bool gameLost;

    public UploadController uploader;


    

    // Start is called before the first frame update
    void Start()
    {
        zhibBase = zhib.GetComponent<CharacterBaseBehavior>();

        neralaBase = nerala.GetComponent<CharacterBaseBehavior>();
        hunterSeekerBase = nerala.GetComponent<HunterSeekerAbility>();
        hunterSeekerActive = false;

        omozraBase = omozra.GetComponent<CharacterBaseBehavior>();

        cameraScript = gameObject.GetComponent<CameraMovement>();

        selectedCharacter = zhib;

        gameLost = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (!gameLost)
        {
            if(zhib != null)
            {
                if (zhibBase.playerHealth == 0)
                {
                    uploader.OnDeath((int)zhibBase.transform.position.x, (int)zhibBase.transform.position.z);
                    if(zhib == cameraScript.focusedPlayer)
                    {
                        if (nerala != null && neralaUnlocked)
                            selectedCharacter = nerala;
                        else if (omozra != null && omozraUnlocked)
                            selectedCharacter = omozra;
                    }

                    Destroy(zhib);
                }
            }

            if(nerala != null)
            {
                if (neralaBase.playerHealth == 0)
                {
                    if (nerala == cameraScript.focusedPlayer)
                    {
                        if (zhib != null)
                            selectedCharacter = zhib;
                        else if (omozra != null && omozraUnlocked)
                            selectedCharacter = omozra;
                    }

                    Destroy(nerala);
                }
            }

            if(omozra != null)
            {
                if (omozraBase.playerHealth == 0)
                {
                    if (omozra == cameraScript.focusedPlayer)
                    {
                        if (zhib != null)
                            selectedCharacter = zhib;
                        else if (nerala != null)
                            selectedCharacter = nerala;
                    }

                    Destroy(omozra);
                    Destroy(sadiq);
                }
            }

            if (zhibBase.playerHealth == 0 && (neralaBase.playerHealth == 0 || !neralaUnlocked) && (omozraBase.playerHealth == 0 || !omozraUnlocked))
                gameLost = true;

            if (Input.GetKey(KeyCode.Z) && !hunterSeekerActive && zhib != null)
            {
                selectedCharacter = zhib;
                allSelected = false;
            }

            if (Input.GetKey(KeyCode.X) && !hunterSeekerActive && neralaUnlocked && nerala != null)
            {
                selectedCharacter = nerala;
                allSelected = false;
            }

            if (Input.GetKey(KeyCode.C) && !hunterSeekerActive && omozraUnlocked && omozra != null)
            {
                selectedCharacter = omozra;
                allSelected = false;
            }

            if (Input.GetKey(KeyCode.V) && !hunterSeekerActive)
            {
                allSelected = true;
                if(zhib != null) zhibBase.allSelected = true;
                if(neralaUnlocked && nerala != null) neralaBase.allSelected = true;
                if(omozraUnlocked && omozra != null) omozraBase.allSelected = true;
            }


            if (hunterSeekerActive && selectedCharacter == null)
            {
                selectedCharacter = nerala;
                hunterSeekerActive = false;
            }


            if (selectedCharacter == nerala)
            {
                if (GameObject.Find("HunterSeeker(Clone)") != null)
                {
                    hunterSeeker = GameObject.Find("HunterSeeker(Clone)");
                    selectedCharacter = hunterSeeker;
                    hunterSeekerActive = true;
                }
            }

            if (!allSelected)
            {
                switch (selectedCharacter.name)
                {
                    case "Zhib":
                        zhibBase.selectedCharacter = true;
                        zhibBase.allSelected = false;

                        neralaBase.selectedCharacter = false;
                        neralaBase.allSelected = false;

                        omozraBase.selectedCharacter = false;
                        omozraBase.allSelected = false;
                        break;

                    case "Nerala":
                        neralaBase.selectedCharacter = true;
                        neralaBase.allSelected = false;

                        zhibBase.selectedCharacter = false;
                        zhibBase.allSelected = false;

                        omozraBase.selectedCharacter = false;
                        omozraBase.allSelected = false;
                        break;

                    case "Omozra":
                        omozraBase.selectedCharacter = true;
                        omozraBase.allSelected = false;

                        neralaBase.selectedCharacter = false;
                        neralaBase.allSelected = false;

                        zhibBase.selectedCharacter = false;
                        zhibBase.allSelected = false;
                        break;

                    case "HunterSeeker(Clone)":
                        neralaBase.selectedCharacter = false;
                        neralaBase.allSelected = false;

                        zhibBase.selectedCharacter = false;
                        zhibBase.allSelected = false;

                        omozraBase.selectedCharacter = false;
                        omozraBase.allSelected = false;
                        break;
                }
            }

            cameraScript.focusedPlayer = selectedCharacter;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if(totalSpice < 0)
        {
            totalSpice = 0;
        }

        //totalSpice = zhibBase.playerSpice + neralaBase.playerSpice + omozraBase.playerSpice;
    }

    void OnGUI()
    {
        if (gameLost)
            GUI.Box(new Rect(Screen.width / 2 - 125, Screen.height / 2, 250, 30), "Press 'R' to restart");
        GUI.Box(new Rect(5, 40, 50, 25), "Spice");
        GUI.Box(new Rect(62, 40, 40, 25), totalSpice.ToString());
    }
}
