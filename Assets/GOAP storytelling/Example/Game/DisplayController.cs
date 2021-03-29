using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayController : MonoBehaviour
{
    public static DisplayController instance = null;

    public Camera gameCamera;
    public GameObject SlotCamera;
    public GameObject displayConsoleText;
    public GameObject displayGoalText;

    public GameObject moodBar;
    public GameObject currentMoodDisplay;
    
    private RenderTexture renderGameScene;
    public float SmallCameraSize = 4f;

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
     
    }

    public void ShowOnConsolePlan(string text)
    {
        displayConsoleText.GetComponent<Text>().text = text;
    }

    public void ShowOnConsole(string text, Color color)
    {
        displayConsoleText.GetComponent<Text>().text = text;
        displayConsoleText.GetComponent<Text>().color = color;
    }
    
    public void ChangeMood(Mood mood, float durationChange, float successChange, float increment)
    {
        mood.bar.value += increment;
        if (mood.bar.value >= mood.threshold)
        {
            npc.GetComponent<BigFivePersonality>().mood = mood.name;
            npc.GetComponent<HogwartsStudent>().durationActionInfluence = durationChange;
            npc.GetComponent<HogwartsStudent>().successActionInfluence = successChange;
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
        ChangeMood(moodDict[MoodType.Joy], 0.5f, 1f, 1f);
    }

    public void ChangeMoodToSad()
    {
        ChangeMood(moodDict[MoodType.Sad], 2f, 1f, 1f);
    }

    public void ChangeMoodToAngry()
    {
        ChangeMood(moodDict[MoodType.Angry], 0.5f, 0.5f, 1f);
    }

    public void ChangeMoodToFear()
    {
        ChangeMood(moodDict[MoodType.Fear], 1f, 1f, 1f);
    }

    public void ChangeMoodToDisgust()
    {
        ChangeMood(moodDict[MoodType.Disgust], 1f, 1f, 1f);
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
                npc.transform.GetComponentInChildren<Canvas>().transform.GetChild(1).transform.GetComponent<Image>().color = new Color(255, 255, 255, 0);
                npc.transform.GetComponentInChildren<Canvas>().transform.GetChild(1).transform.GetComponent<Image>().sprite = null;
                currentMoodDisplay.GetComponent<Image>().sprite = moodDict[MoodType.Neutral].sprite;
                currentMoodDisplay.GetComponent<Image>().color = moodDict[MoodType.Neutral].color;
            }
        }
    }
}
