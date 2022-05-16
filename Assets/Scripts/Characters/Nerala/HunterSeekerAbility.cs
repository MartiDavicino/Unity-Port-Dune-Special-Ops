using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterSeekerAbility : MonoBehaviour
{
    private CharacterBaseBehavior baseScript;
    private bool addLineComponentOnce;
    private float spawnRange;
    private bool hunterDeployed;

    public float hunterSeekerVelocity;
    public float hunterSeekerMaxRange;
    public float soundRange;
    public float soundMultiplier;
    public float countdownTime;

    public GameObject hunterSeekerPrefab;
    public GameObject spicePrefab;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = GetComponent<CharacterBaseBehavior>();
        spawnRange = 3f;

        hunterDeployed = false;
        addLineComponentOnce = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (hunterDeployed || baseScript.state == PlayerState.ABILITY3)
        {
            baseScript.state = PlayerState.IDLE;
            hunterDeployed = false;
        }

        if (baseScript.selectedCharacter)
        {
            if (baseScript.ability3Active)
            {
                if (addLineComponentOnce)
                {
                    addLineComponentOnce = false;
                    gameObject.AddComponent<LineRenderer>();
                }

                gameObject.DrawCircleScaled(spawnRange, 0.05f, transform.localScale);

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Camera.main.GetComponent<GeneralManager>().totalSpice -= baseScript.ultimateCost;

                    Vector3 spawnPoint = gameObject.transform.position + gameObject.transform.forward * spawnRange + gameObject.transform.up * 1;
                    Instantiate(hunterSeekerPrefab, spawnPoint, gameObject.transform.rotation);

                    baseScript.hunterSeeking = true;

                    hunterDeployed = true;

                    baseScript.state = PlayerState.ABILITY3;
                }
            } else
            {
                addLineComponentOnce = true;
            }
        }

        if (baseScript.hunterSeeking) gameObject.DrawCircleScaled(hunterSeekerMaxRange, 0.05f, transform.localScale);

    }

    void OnGUI()
    {
        if (baseScript.selectedCharacter)
            if (baseScript.ability3Active) GUI.Box(new Rect(5, Screen.height - 30, 150, 25), "Hunter Seeker Active");
    }
}
