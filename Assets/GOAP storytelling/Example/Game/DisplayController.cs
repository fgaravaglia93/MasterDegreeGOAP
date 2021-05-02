using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayController : MonoBehaviour
{
    public static DisplayController instance = null;
    
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
    [HideInInspector]
    
   /* private RenderTexture renderGameScene;
    public float SmallCameraSize = 4f;*/

    //to redefine later
    public GameObject npc;

    private Mood currentMood;
    private int cooldownSteps = 5; //default with no personality affection
    
    private string spritePathUI = "Sprites/ui_expression_";
    
    [HideInInspector]
    public Dictionary<MoodType, Mood> moodDict = new Dictionary<MoodType, Mood>();

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
                    //Debug.Log("Bar" + moodName);
                    moodDict.Add(moodName, new Mood(moodName, spritePathUI+moodName, 
                        t.GetComponentInChildren<Slider>(), npc.GetComponent<BigFivePersonality>().MoodSwitchThreshold(moodName)));
                    break;
                }
            }
        }

        moodDict.Add(MoodType.Neutral, new Mood(spritePathUI+MoodType.Neutral));

        Camera [] cameras = (Camera [])FindObjectsOfType(typeof(Camera));
        
        foreach(Camera cam in cameras)
        {
            cam.enabled = false;
        }

        gameCamera.enabled = true;
    }

    private void Update()
    {
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

                    foreach (MoodType moodName in System.Enum.GetValues(typeof(MoodType)))
                    {
                        if (moodName != MoodType.Neutral)
                        {
                            moodDict[moodName].threshold = npc.GetComponent<BigFivePersonality>().thresholdMoodValues[moodName];
                            moodDict[moodName].bar.value = npc.GetComponent<BigFivePersonality>().currentMoodValues[moodName];
                            moodDict[moodName].SetPlaceholder(moodDict[moodName].threshold);
                        }
                    }

                    if (npc.GetComponent<PersonalityAgent>() != null)
                    {
                        displayGOAP.gameObject.SetActive(true);
                        npc.GetComponent<PersonalityAgent>().displayed = true;
                        //update console values
                        ShowOnConsolePlan(npc.GetComponent<PersonalityAgent>().planListText);
                        ShowOnConsoleAction(npc.GetComponent<PersonalityAgent>().actionText, npc.GetComponent<PersonalityAgent>().actionColor);
                        ShowOnConsoleGoal(npc.GetComponent<PersonalityAgent>().goalText, npc.GetComponent<PersonalityAgent>().goalColor);
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
            if(npc.GetComponent<PersonalityAgent>()!=null)
             npc.GetComponent<PersonalityAgent>().displayed = false;

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

    public void ChangeMood(Mood mood, float increment)
    {   
        npc.GetComponent<BigFivePersonality>().listenerChange = true;
        npc.GetComponent<BigFivePersonality>().moodActivation = mood.name;
        npc.GetComponent<BigFivePersonality>().incrementMood = increment;
    }

    public void ChangeMood(MoodType moodType, float increment)
    {
        if (moodType == MoodType.Neutral)
            ChangeMood(MoodType.Neutral, increment);
        else
            ChangeMood(moodDict[moodType], increment);
    }


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
