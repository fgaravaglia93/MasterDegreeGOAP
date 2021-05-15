﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayManager : MonoBehaviour
{
    public static DisplayManager instance = null;
    
    //defaultCamera = HeroCamera
    public Camera gameCamera;
    
    public GameObject displayBox;
    public GameObject dialogueBox;
    public GameObject displayGOAP;
    public GameObject displayOCEAN;

    [HideInInspector]
    public Camera npcOverlayCamera;
    public GameObject displayConsoleText;
    public GameObject displayGoalText;
    public GameObject displayActionText;

    public GameObject moodBar;
    public GameObject currentMoodDisplay;
    public GameObject cursor;
 
    public GameObject npc;

    private MoodBar currentMood;
    private int cooldownSteps = 5; //default with no personality affection
    
    private string spritePathUI = "Sprites/ui_expression_";
    
    [HideInInspector]
    public Dictionary<MoodType, MoodBar> moodDict = new Dictionary<MoodType, MoodBar>();

    //Overlay
    [HideInInspector]
    public bool overlayInUse = false;
    [HideInInspector]
    public bool interact;

    //Used to lock emotions
    [HideInInspector]
    public bool lockMood = false;
    Vector2 pos;
    RaycastHit2D hit;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        displayBox.SetActive(false);
        dialogueBox.SetActive(false);
        Cursor.visible = false;
        
        interact = false;
    }

    void Start()
    {
        var moodNames = System.Enum.GetNames(typeof(MoodType));
 
        //check and instantiate Mood objects to the relative sliders in the Scene UI
        foreach (MoodType moodName in System.Enum.GetValues(typeof(MoodType)))
        {
            foreach (Transform t in moodBar.transform)
            {
                if (t.name == "Bar" + moodName)
                {
                    moodDict.Add(moodName, new MoodBar(moodName, spritePathUI + moodName,
                        t.GetComponentInChildren<Slider>(), npc.GetComponent<MoodController>().MoodSwitchThreshold(moodName, npc.GetComponent<MoodController>().model)));
                    break;
                }
            }
        }

        moodDict.Add(MoodType.Neutral, new MoodBar(spritePathUI+MoodType.Neutral));

        Camera [] cameras = (Camera [])FindObjectsOfType(typeof(Camera));
        
        foreach(Camera cam in cameras)
        {
            cam.enabled = false;
        }

        gameCamera.enabled = true;
    }

    private void Update()
    {

        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursor.transform.position = cursorPos;
        //mouse over NPC
        if (Input.GetMouseButtonDown(0) && !overlayInUse)
        {
            pos = Input.mousePosition;
            pos = gameCamera.ScreenToWorldPoint(pos);
            hit = Physics2D.Raycast(pos, Vector2.zero);

            if (!interact)
            {
                if (hit && (hit.collider.gameObject.CompareTag("NPC") || hit.collider.transform.parent.gameObject.CompareTag("NPC")))
                {
                    Debug.Log("Click");
                    displayBox.gameObject.SetActive(true);
                    npc = hit.transform.gameObject;
                    npcOverlayCamera = npc.GetComponentInChildren<Camera>();
                    npcOverlayCamera.enabled = true;
                    gameCamera.enabled = false;
                    displayGOAP.gameObject.SetActive(false);
                    ShowOnConsolePersonality();
                    displayOCEAN.transform.parent.gameObject.SetActive(true);
                    moodBar.transform.parent.gameObject.SetActive(true);

                    foreach (MoodType moodName in System.Enum.GetValues(typeof(MoodType)))
                    {
                        if (moodName != MoodType.Neutral)
                        {
                            moodDict[moodName].threshold = npc.GetComponent<MoodController>().thresholdMoodValues[moodName];
                            moodDict[moodName].bar.value = npc.GetComponent<MoodController>().currentMoodValues[moodName];
                            moodDict[moodName].SetPlaceholder(moodDict[moodName].threshold);
                        }
                    }

                    if (npc.GetComponent<Moody5Agent>() != null)
                    {
                        displayGOAP.gameObject.SetActive(true);
                        npc.GetComponent<Moody5Agent>().displayed = true;
                        //update console values
                        ShowOnConsolePlan(npc.GetComponent<Moody5Agent>().planListText);
                        ShowOnConsoleAction(npc.GetComponent<Moody5Agent>().actionText, npc.GetComponent<Moody5Agent>().actionColor);
                        ShowOnConsoleGoal(npc.GetComponent<Moody5Agent>().goalText, npc.GetComponent<Moody5Agent>().goalColor);
                    }

                    overlayInUse = true;
                }
            }
          
        }

        else if (Input.GetButtonDown("Escape") && overlayInUse)
        {
            overlayInUse = false;
            displayBox.gameObject.SetActive(false);
            gameCamera.enabled = true;
            npcOverlayCamera.enabled = false;
            if(npc.GetComponent<Moody5Agent>()!=null)
                npc.GetComponent<Moody5Agent>().displayed = false;
        }
        else { }
    }


    public void ShowOnConsolePlan(string text)
    {
        displayConsoleText.GetComponent<Text>().text = text;
        displayConsoleText.GetComponent<Text>().color = new Color(1f,1f,1f);
    }

    public void ShowOnConsolePlan(string text, Color color)
    {
        displayConsoleText.GetComponent<Text>().text = text;
        displayConsoleText.GetComponent<Text>().color = color;
    }

    public void ShowOnConsoleAction(string text)
    {
        displayActionText.GetComponent<Text>().text = text;
        displayActionText.GetComponent<Text>().color = new Color(255, 255, 255);
    }

    public void ShowOnConsoleAction(string text, Color color)
    {
        displayActionText.GetComponent<Text>().text = text;
        displayActionText.GetComponent<Text>().color = color;
    }

    public void ShowOnConsoleGoal(string text)
    {
        displayGoalText.GetComponent<Text>().text = text;
        displayGoalText.GetComponent<Text>().color = new Color(255, 255, 255);
    }

    public void ShowOnConsoleGoal(string text, Color color)
    {
        displayGoalText.GetComponent<Text>().text = text;
        displayGoalText.GetComponent<Text>().color = color;
    }

    public void ShowOnConsolePersonality()
    {
        displayOCEAN.transform.GetChild(0).GetComponent<Text>().text = "" +npc.GetComponent<BigFivePersonality>().openness;
        displayOCEAN.transform.GetChild(1).GetComponent<Text>().text = "" + npc.GetComponent<BigFivePersonality>().consciousness;
        displayOCEAN.transform.GetChild(2).GetComponent<Text>().text = "" + npc.GetComponent<BigFivePersonality>().extraversion;
        displayOCEAN.transform.GetChild(3).GetComponent<Text>().text = "" + npc.GetComponent<BigFivePersonality>().agreeableness;
        displayOCEAN.transform.GetChild(4).GetComponent<Text>().text = "" + npc.GetComponent<BigFivePersonality>().neuroticism;
    } 

    public void ChangeMood(MoodBar mood, float increment)
    {   
        npc.GetComponent<MoodController>().listenerChange = true;
        npc.GetComponent<MoodController>().moodActivation = mood.name;
        npc.GetComponent<MoodController>().incrementMood = increment;
    }

    public void ChangeMood(MoodType moodType, float increment)
    {
        if (moodType == MoodType.Neutral)
            ChangeMood(MoodType.Neutral, increment);
        else
            ChangeMood(moodDict[moodType], increment);
    }

 
    //Cursor

    //TO REMOVE (Used to test by buttons)

    public void ChangeMoodToJoy()
    {
        ChangeMood(moodDict[MoodType.Joy], 1f);
    }

    public void ChangeMoodToSad()
    {
        ChangeMood(moodDict[MoodType.Sad], 1f);
    }

    public void ChangeMoodToAngry()
    {
        ChangeMood(moodDict[MoodType.Angry], 1f);
    }

    public void ChangeMoodToFear()
    {
        ChangeMood(moodDict[MoodType.Fear], 1f);
    }

    public void ChangeMoodToDisgust()
    {
        ChangeMood(moodDict[MoodType.Disgust], 1f);
    }

    public void DisplayChange()
    {
        print("Il PG è felice");
    }
    
    




}