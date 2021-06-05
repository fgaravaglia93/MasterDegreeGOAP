using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MoodController : MonoBehaviour
{
    public MoodType mood = MoodType.Neutral;
    private float bigFiveInfluence = 0.2f;

    [HideInInspector]
    public BigFivePersonality model;

    private string spritePathBaloon = "Sprites/ui_baloon_";
    private float stepCooldown = 3f;
    private int cooldownSteps = 5; //default with no personality affection
    [HideInInspector]
    public Dictionary<MoodType, float> currentMoodValues = new Dictionary<MoodType, float>();
    [HideInInspector]
    public Dictionary<MoodType, float> thresholdMoodValues = new Dictionary<MoodType, float>();
    
    [HideInInspector]
    public bool lockMood = false;
    [HideInInspector]
    public bool listenerChange;
    [HideInInspector]
    public MoodType moodActivation;
    [HideInInspector]
    public float incrementMood;
    [HideInInspector]
    public GameObject containerUI;
    [HideInInspector]
    public GameObject baloon;
    [HideInInspector]
    public string textTrait;
    [HideInInspector]
    public Color colorTrait;

    void Awake()
    {
        model = GetComponent<BigFivePersonality>();
        if (GetComponentInChildren<ParticleSystem>() != null)
            GetComponentInChildren<ParticleSystem>().Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Set up mood at the start
        currentMoodValues.Add(MoodType.Joy, 0);
        currentMoodValues.Add(MoodType.Sadness, 0);
        currentMoodValues.Add(MoodType.Angry, 0);
        currentMoodValues.Add(MoodType.Fear, 0);
        currentMoodValues.Add(MoodType.Disgust, 0);
        //Set up threshold
        thresholdMoodValues.Add(MoodType.Joy, MoodSwitchThreshold(MoodType.Joy, model));
        thresholdMoodValues.Add(MoodType.Sadness, MoodSwitchThreshold(MoodType.Sadness ,model));
        thresholdMoodValues.Add(MoodType.Angry, MoodSwitchThreshold(MoodType.Angry, model));
        thresholdMoodValues.Add(MoodType.Fear, MoodSwitchThreshold(MoodType.Fear, model));
        thresholdMoodValues.Add(MoodType.Disgust, MoodSwitchThreshold(MoodType.Disgust, model));
        moodActivation = MoodType.Joy;
        listenerChange = false;
        var children = GetComponentsInChildren<Transform>();

        var container = children.Where(child => child.tag == "ContainerUI").ToArray();
        if (container != null)
            containerUI = container[0].gameObject;

        var baloons = children.Where(child => child.tag == "Baloon").ToArray();
        if (baloons != null)
            baloon = baloons[0].gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        containerUI.transform.position = Camera.main.WorldToScreenPoint(transform.position);

        if (listenerChange)
        {
            if (!lockMood)
            {
                var moodRef = DisplayManager.instance.moodDict[moodActivation];
                currentMoodValues[moodActivation] += incrementMood;
                //Debug.Log(this.gameObject.name + " - increase - " + currentMoodValues[moodActivation] + " - threshold - " + thresholdMoodValues[moodActivation]);

                if (DisplayManager.instance.npc == transform.gameObject)
                {
                    moodRef.bar.value += incrementMood;
                }

                if (currentMoodValues[moodActivation] >= thresholdMoodValues[moodActivation])
                {
                    mood = moodActivation;
                    GetComponent<HogwartsStudent>().durationActionInfluence = moodRef.durationChange;
                    GetComponent<HogwartsStudent>().successActionInfluence = moodRef.successChange;
                    baloon.transform.GetComponent<Image>().sprite = Resources.Load<Sprite>(spritePathBaloon + moodRef.name);
                    baloon.transform.GetComponent<Image>().color = new Color(255, 255, 255, 0.75f);
                    StopCoroutine("CooldownEmotion");
                    StartCoroutine("CooldownEmotion");
                    //is this displayed?
                    if (DisplayManager.instance.npc == this)
                    {
                        DisplayManager.instance.currentMoodDisplay.GetComponent<Image>().sprite = moodRef.sprite;
                        DisplayManager.instance.currentMoodDisplay.GetComponent<Image>().color = moodRef.color;
                    }

                }
                listenerChange = false;

                //To define specific behavior for NPCs based on the current Mood
                //MoodBehavior();
            }
        }
    }

    IEnumerator CooldownEmotion()
    {
        var moodToCooldown = moodActivation;
        int cooldownSteps = (int)currentMoodValues[moodToCooldown];
        int i = 0;
        while (i < cooldownSteps)
        {
            yield return new WaitForSeconds(stepCooldown);
            if (!lockMood)
            {
                i++;
                currentMoodValues[moodToCooldown] -= incrementMood;
                //Debug.Log(this.gameObject.name + " - cooldown - " + currentMoodValues[moodToCooldown]);
                var moodRef = DisplayManager.instance.moodDict[moodToCooldown];
                if (DisplayManager.instance.npc == transform.gameObject) { }
                moodRef.bar.value -= incrementMood;
                if (currentMoodValues[moodToCooldown] < thresholdMoodValues[moodToCooldown])
                {
                    mood = MoodType.Neutral;
                    GetComponent<HogwartsStudent>().durationActionInfluence = 1f;
                    GetComponent<HogwartsStudent>().successActionInfluence = 1f;
                    baloon.transform.GetComponent<Image>().color = new Color(255, 255, 255, 0);
                    baloon.transform.GetComponent<Image>().sprite = null;
                    if (DisplayManager.instance.npc == this)
                    {
                        DisplayManager.instance.currentMoodDisplay.GetComponent<Image>().sprite = DisplayManager.instance.moodDict[MoodType.Neutral].sprite;
                        DisplayManager.instance.currentMoodDisplay.GetComponent<Image>().color = DisplayManager.instance.moodDict[MoodType.Neutral].color;
                    }
                }
            }
        }
    }

    //Calculate value of each mood threshold depending on the personality model OCEAN 
    public float MoodSwitchThreshold(MoodType mood, BigFivePersonality model)
    {

        //default value with no personality
        float threshold = 5f;
        float maxBar = 10f;
        switch (mood)
        {
            case MoodType.Joy:
                threshold -= (model.neuroticism * maxBar * bigFiveInfluence + model.extraversion * maxBar * bigFiveInfluence);
                break;
            case MoodType.Sadness:
                threshold -= model.neuroticism * maxBar * bigFiveInfluence;
                break;
            case MoodType.Angry:
                threshold -= (model.neuroticism * maxBar * bigFiveInfluence - model.agreeableness * maxBar * bigFiveInfluence);
                break;
            case MoodType.Fear:
                threshold -= model.neuroticism * maxBar * bigFiveInfluence;
                break;
            case MoodType.Disgust:
                threshold -= (model.neuroticism * maxBar * bigFiveInfluence - model.agreeableness * maxBar * bigFiveInfluence);
                break;
            default:
                break;
        }
        return threshold;
    }

    //Extend to define specific NPC behavior in the current mood
    private void MoodBehavior()
    {
        switch (mood)
        {
            case MoodType.Joy:
                //Define Joy Behavior
                break;
            case MoodType.Sadness:
                //Define Sadness Behavior
                break;
            case MoodType.Angry:
                //Define Angry Behavior
                break;
            case MoodType.Fear:
                //Define Fear Behavior
                break;
            case MoodType.Disgust:
                //define Disgust Behavior
                break;
            default:
                break;
        }
    }
       

    private void OnMouseOver()
    {
        DisplayManager.instance.cursor.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/ui_cursor_investigate");
    }
    private void OnMouseExit()
    {
        DisplayManager.instance.cursor.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/ui_cursor");
    }

}
