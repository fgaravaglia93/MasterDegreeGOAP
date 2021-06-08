using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DisplayManager : MonoBehaviour
{
    public static DisplayManager instance = null;
    
    //defaultCamera = HeroCamera
    public Camera gameCamera;
    
    public GameObject displayBox;
    public GameObject dialogueBox;
    public GameObject displayGOAP;
    public GameObject displayOCEAN;
    public GameObject displaySwitchMood;

    [HideInInspector]
    public Camera npcOverlayCamera;
    public GameObject displayConsoleText;
    public GameObject displayGoalText;
    public GameObject displayActionText;
    public GameObject displayTraitAdhoc;
    public GameObject moodBar;
    public GameObject currentMoodDisplay;
    public GameObject cursor;
    public GameObject npc;

    [Range(1,10)]
    public int moodIntensity;

    private MoodBar currentMood;
    private int cooldownSteps = 5; //default with no personality affection
    //Manage baloons sprite
    private string spritePathUI = "Sprites/ui_expression_";
    //Manage facial expression codes
    private string spritePathFace = "Dialogue/Faceset/";

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
                    displayTraitAdhoc.gameObject.SetActive(true);
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

        else if (Input.GetButtonDown("back") && overlayInUse)
        {
            overlayInUse = false;
            displayBox.gameObject.SetActive(false);
            displayTraitAdhoc.gameObject.SetActive(false);
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
        displayActionText.GetComponent<Text>().text = /*"Action : " + */text;
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

    public void ShowOnConsoleTraitAdHoc(string text)
    {
        displayTraitAdhoc.GetComponent<Text>().text = text;
    }

    public void ShowOnConsoleTraitAdHoc(string text, Color color)
    {
        displayTraitAdhoc.GetComponent<Text>().text = text;
        displayTraitAdhoc.GetComponent<Text>().color = color;
    }

    public void ShowOnConsolePersonality()
    {
        displayOCEAN.transform.GetChild(0).GetComponent<Text>().text = "" +npc.GetComponent<BigFivePersonality>().openness;
        displayOCEAN.transform.GetChild(1).GetComponent<Text>().text = "" + npc.GetComponent<BigFivePersonality>().conscientiousness;
        displayOCEAN.transform.GetChild(2).GetComponent<Text>().text = "" + npc.GetComponent<BigFivePersonality>().extraversion;
        displayOCEAN.transform.GetChild(3).GetComponent<Text>().text = "" + npc.GetComponent<BigFivePersonality>().agreeableness;
        displayOCEAN.transform.GetChild(4).GetComponent<Text>().text = "" + npc.GetComponent<BigFivePersonality>().neuroticism;
    } 

    public void ChangeMood(GameObject npcMood, MoodBar mood, float increment)
    {   
        npcMood.GetComponent<MoodController>().listenerChange = true;
        npcMood.GetComponent<MoodController>().moodActivation = mood.name;
        npcMood.GetComponent<MoodController>().incrementMood = increment;
        npcMood.GetComponent<DialogueController>().face = GetFace(npc.GetComponent<DialogueController>().face.name, (int)npc.GetComponent<MoodController>().mood);

    }

    public void ChangeMood(GameObject npcMood, MoodType moodType, float increment)
    {
        if (moodType == MoodType.Neutral)
            ChangeMood(npcMood, MoodType.Neutral, increment);
        else
            ChangeMood(npcMood, moodDict[moodType], increment);
    }

    //Get NPC Face expression associated to a moodindex
    public Sprite GetFace(string rootName, int code)
    {
        rootName = rootName.Remove(rootName.Length - 2);
        Sprite[] all = Resources.LoadAll<Sprite>(spritePathFace + rootName);
        //Debug.Log(all[code].name);
        return all[code];
    }
    //TO REMOVE (Used to test by buttons)

    public void ChangeMoodToJoy()
    {
        ChangeMood(npc, moodDict[MoodType.Joy], moodIntensity);
    }

    public void ChangeMoodToSadness()
    {
        ChangeMood(npc, moodDict[MoodType.Sadness], moodIntensity);
    }

    public void ChangeMoodToAngry()
    {
        ChangeMood(npc, moodDict[MoodType.Angry], moodIntensity);
    }

    public void ChangeMoodToFear()
    {
        ChangeMood(npc, moodDict[MoodType.Fear], moodIntensity);
    }

    public void ChangeMoodToDisgust()
    {
        ChangeMood(npc, moodDict[MoodType.Disgust], moodIntensity);
    }

    
}
