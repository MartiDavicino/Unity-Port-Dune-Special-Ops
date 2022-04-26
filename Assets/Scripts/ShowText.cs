using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowText : MonoBehaviour
{

    public ActiveQuest nextObjective;

    public string text;
    private bool showText;
    // Start is called before the first frame update
    void Start()
    {
        showText = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            showText = true;
            GameObject go = GameObject.Find("playercamera");

            GeneralManager manager = go.GetComponent<GeneralManager>();

            if (other.gameObject.name == "Nerala" && manager.neralaUnlocked)
            {
                if(nextObjective != ActiveQuest.NONE)
                {
                    QuestManager qManager = go.GetComponent<QuestManager>();
                    qManager.activeQuest = nextObjective;
                    Destroy(gameObject);
                }
            } else if (other.gameObject.name != "Nerala")
            {
                if (nextObjective != ActiveQuest.NONE)
                {
                    QuestManager qManager = go.GetComponent<QuestManager>();
                    qManager.activeQuest = nextObjective;
                    Destroy(gameObject);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            showText = false;
    }
    private void OnGUI()
    {
        if(showText && nextObjective == ActiveQuest.NONE)
            GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 250, 30), text);
    }
}
