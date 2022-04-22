using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterSeekerAbility : MonoBehaviour
{
    public CharacterBaseBehavior baseScript;

    private Animator neralaAnimator;

    private bool addLineComponentOnce;

    public float spawnRange;

    public GameObject hunterSeekerPrefab;

    public bool seekerHunting;

    // Start is called before the first frame update
    void Start()
    {
        addLineComponentOnce = true;

        neralaAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (baseScript.selectedCharacter)
        {
            if (baseScript.ability3Active)
            {
                if (addLineComponentOnce)
                {
                    addLineComponentOnce = false;
                    gameObject.AddComponent<LineRenderer>();
                }

                if(!seekerHunting) gameObject.DrawCircle(spawnRange * 6, .05f);

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Vector3 spawnPoint = gameObject.transform.position + gameObject.transform.forward * spawnRange + gameObject.transform.up * 1;
                    Instantiate(hunterSeekerPrefab, spawnPoint, gameObject.transform.rotation);
                    Destroy(gameObject.GetComponent<LineRenderer>());
                    seekerHunting = true;
                    if (neralaAnimator != null)
                    {
                        neralaAnimator.SetTrigger("mosquito");
                    }
                }
            } else
            {
                addLineComponentOnce = true;
            }
        }
    }

    void OnGUI()
    {
        if (baseScript.selectedCharacter)
            if (baseScript.ability3Active) GUI.Box(new Rect(0, Screen.height - 25, 150, 25), "Hunter Seeker Active");
    }
}
