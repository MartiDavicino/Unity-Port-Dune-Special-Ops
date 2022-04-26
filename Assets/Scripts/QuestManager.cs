using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActiveQuest
{
    NERALA_RESCUE,
    GET_TO_BASE,
    FIND_OTHER,
    WATER_TANK,
    REACH_RABBAN,
    NONE,
}
public class QuestManager : MonoBehaviour
{

    public ActiveQuest activeQuest;

    private float width;
    private float height;
    private float offset;

    void Start()
    {
        width = 200f;
        height = 40f;
        offset = 5f;
    }
    void OnGUI()
    {
        switch(activeQuest)
        {
            case ActiveQuest.NERALA_RESCUE:
                GUI.Box(new Rect(Screen.width - width - offset, 0 + offset, width, height), "Current Objective:\nRescue Nerala");
                break;
            case ActiveQuest.GET_TO_BASE:
                GUI.Box(new Rect(Screen.width - width - offset, 0 + offset, width, height), "Current Objective:\nGet to the Base");
                break;
            case ActiveQuest.FIND_OTHER:
                GUI.Box(new Rect(Screen.width - width - offset, 0 + offset, width, height), "Current Objective:\nFind another way to enter");
                break;
            case ActiveQuest.WATER_TANK:
                GUI.Box(new Rect(Screen.width - width - offset, 0 + offset, width, height), "Current Objective:\nUse the Water Tank");
                break;
            case ActiveQuest.REACH_RABBAN:
                GUI.Box(new Rect(Screen.width - width - offset, 0 + offset, width, height + 15), "Current Objective:\nInfiltrate the base\nand kill Rabban");
                break;
            case ActiveQuest.NONE:
                break;
        }
    }
}
