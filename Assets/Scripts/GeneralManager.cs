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
    public bool neralaUnlocked;

    private GameObject hunterSeeker;
    private HunterSeekerAbility hunterSeekerBase;
    private bool hunterSeekerActive;

    public GameObject omozra;
    private CharacterBaseBehavior omozraBase;

    private GameObject selectedCharacter;
    private bool allSelected;

    private CameraMovement cameraScript;

    public bool gameLost;

    [HideInInspector] public int totalSpice;

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
            if (zhibBase.playerHealth == 0)
            {
                Destroy(zhib);
                if (nerala != null) selectedCharacter = nerala;
                if (omozra != null) selectedCharacter = omozra;
            }

            if (neralaBase.playerHealth == 0)
            {
                Destroy(nerala);
                if (zhib != null) selectedCharacter = zhib;
                if (omozra != null) selectedCharacter = omozra;
            }

            if (neralaBase.playerHealth == 0)
            {
                Destroy(omozra);
                if (zhib != null) selectedCharacter = zhib;
                if (nerala != null) selectedCharacter = nerala;
            }

            if (zhibBase.playerHealth == 0 && neralaBase.playerHealth == 0)
                gameLost = true;

            if (Input.GetKey(KeyCode.Z) && !hunterSeekerActive)
            {
                selectedCharacter = zhib;
                allSelected = false;
            }

            if (Input.GetKey(KeyCode.X) && !hunterSeekerActive && neralaUnlocked)
            {
                selectedCharacter = nerala;
                allSelected = false;
            }

            if (Input.GetKey(KeyCode.C) && !hunterSeekerActive)
            {
                selectedCharacter = omozra;
                allSelected = false;
            }

            if (Input.GetKey(KeyCode.V) && !hunterSeekerActive && neralaUnlocked)
            {
                allSelected = true;
                zhibBase.allSelected = true;
                neralaBase.allSelected = true;
                omozraBase.allSelected = true;
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

        totalSpice = zhibBase.playerSpice + neralaBase.playerSpice + omozraBase.playerSpice;
    }

    void OnGUI()
    {
        if (gameLost)
            GUI.Box(new Rect(Screen.width / 2 - 125, Screen.height / 2, 250, 30), "Press 'R' to restart");
        GUI.Box(new Rect(5, 40, 50, 25), "Spice");
        GUI.Box(new Rect(62, 40, 40, 25), totalSpice.ToString());
    }
}
