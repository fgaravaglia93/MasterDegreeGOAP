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

    [HideInInspector]
    public Camera npcOverlayCamera;
    public GameObject displayConsoleText;
    public GameObject displayGoalText;
    public GameObject displayActionText;

    public GameObject moodBar;
    public GameObject currentMoodDisplay;

    
   /* private RenderTexture renderGameScene;
    public float SmallCameraSize = 4f;*/

    //to redefine later
    public GameObject npc;

    private Mood currentMood;
    private int cooldownSteps = 5; //default with no personality affection
    private float stepCooldown = 1.5f;
    private int countClicks;
    private string spritePathUI = "Sprites/ui_expression_";
    private string spritePathBaloon= "Sprites/ui_baloon_";
    [HideInInspector]
    public Dictionary<MoodType, Mood> moodDict = new Dictionary<MoodType, Mood>();

    //Verlay
    [HideInInspector]
    public bool overlayInUse = false;

    Vector2 pos;
    RaycastHit2D hit;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
           /* renderGameScene = new RenderTexture(350, 300, 24, RenderTextureFormat.ARGB32);
            renderGameScene.Create();
            gameCamera.GetComponent<Camera>().targetTexture = renderGameScene;
            renderGameScene.name = "RT_Game";*/
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
        //npc.GetComponent<PersonalityAgent>().MoodSwitchThreshold(MoodType.Joy)
        countClicks = 0;
        /*SlotCamera.GetComponent<RawImage>().texture = renderGameScene;
        gameCamera.orthographicSize = SmallCameraSize;*/
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
            if (hit && (hit.collider.gameObject.CompareTag("NPC") || hit.collider.transform.parent.gameObject.CompareTag("NPC")))
            {
                displayBox.gameObject.SetActive(true);
                npcOverlayCamera = hit.transform.gameObject.GetComponentInChildren<Camera>();
                npcOverlayCamera.enabled = true;
                gameCamera.enabled = false;
                displayGOAP.gameObject.SetActive(false);
                if (hit.collider.GetComponent<PersonalityAgent>()!=null)
                {
                    displayGOAP.gameObject.SetActive(true);
                }
                overlayInUse = true;
            }
        }

        else if (Input.GetButtonDown("Escape") && overlayInUse)
        {
            overlayInUse = false;
            displayBox.gameObject.SetActive(false);
            gameCamera.enabled = true;
            npcOverlayCamera.enabled = false;
        }
        else { }
    }


    public void ShowOnConsolePlan(string text)
    {
        displayConsoleText.GetComponent<Text>().text = text;
        displayConsoleText.GetComponent<Text>().color = new Color(255,255,255);
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

    public void ChangeMood(Mood mood, float increment)
    {
        mood.bar.value += increment;
        if (mood.bar.value >= mood.threshold)
        {
            //Debug.Log("Change Angry");
            npc.GetComponent<BigFivePersonality>().mood = mood.name;
            npc.GetComponent<HogwartsStudent>().durationActionInfluence = mood.durationChange;
            npc.GetComponent<HogwartsStudent>().successActionInfluence = mood.successChange;
            //npc.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePathBaloon+mood.name);
            npc.transform.GetComponentInChildren<Canvas>().transform.GetChild(1).transform.GetComponent<Image>().sprite = Resources.Load<Sprite>(spritePathBaloon + mood.name);
            npc.transform.GetComponentInChildren<Canvas>().transform.GetChild(1).transform.GetComponent<Image>().color = new Color(255, 255, 255, 0.75f);
            currentMoodDisplay.GetComponent<Image>().sprite = mood.sprite;
            currentMoodDisplay.GetComponent<Image>().color = mood.color;
            currentMood = mood;
            StopCoroutine("CooldownEmotion");
            StartCoroutine("CooldownEmotion");
        }
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

    IEnumerator CooldownEmotion()
    {
        cooldownSteps = (int)currentMood.bar.value;
        for (int i = 0; i < cooldownSteps; i++)
        {
            //Debug.Log(i);
            yield return new WaitForSeconds(stepCooldown);
            currentMood.bar.value -= 1f;
            if (currentMood.bar.value < currentMood.threshold)
            {
                npc.GetComponent<BigFivePersonality>().mood = MoodType.Neutral;
                npc.GetComponent<HogwartsStudent>().durationActionInfluence = 1f;
                npc.GetComponent<HogwartsStudent>().successActionInfluence = 1f;
                npc.transform.GetComponentInChildren<Canvas>().transform.GetChild(1).transform.GetComponent<Image>().color = new Color(255, 255, 255, 0);
                npc.transform.GetComponentInChildren<Canvas>().transform.GetChild(1).transform.GetComponent<Image>().sprite = null;
                currentMoodDisplay.GetComponent<Image>().sprite = moodDict[MoodType.Neutral].sprite;
                currentMoodDisplay.GetComponent<Image>().color = moodDict[MoodType.Neutral].color;
            }
        }
    }

    public void DisplayChange()
    {
        print("Il PG è felice");
    }
}
