using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum MoodType { Neutral, Joy, Sad, Angry, Fear, Disgust }

public class BigFivePersonality : MonoBehaviour
{
    private string spritePathBaloon = "Sprites/ui_baloon_";

    [Space(10)]
    [Header("BIG 5 personality model")]

    [Tooltip("Openness to Experience.\nAgent preference on new actions")]
    [Range(-1, 1)]
    public int openness;
    [Range(-1, 1)]
    public int consciousness;
    [Range(-1, 1)]
    public int extraversion;
    [Range(-1, 1)]
    public int agreeableness;
    [Range(-1, 1)]
    public int neuroticism;

    private float stepCooldown = 3f;
    private int cooldownSteps = 5; //default with no personality affection
    [HideInInspector]
    public Dictionary<MoodType, float> currentMoodValues = new Dictionary<MoodType, float>();
    [HideInInspector]
    public Dictionary<MoodType, float> thresholdMoodValues = new Dictionary<MoodType, float>();
    public MoodType mood = MoodType.Neutral;
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

    void Start()
    {
        //Set up mood at the start
        currentMoodValues.Add(MoodType.Joy, 0);
        currentMoodValues.Add(MoodType.Sad, 0);
        currentMoodValues.Add(MoodType.Angry, 0);
        currentMoodValues.Add(MoodType.Fear, 0);
        currentMoodValues.Add(MoodType.Disgust, 0);
        //Set up threshold
        thresholdMoodValues.Add(MoodType.Joy, MoodSwitchThreshold(MoodType.Joy));
        thresholdMoodValues.Add(MoodType.Sad, MoodSwitchThreshold(MoodType.Sad));
        thresholdMoodValues.Add(MoodType.Angry, MoodSwitchThreshold(MoodType.Angry));
        thresholdMoodValues.Add(MoodType.Fear, MoodSwitchThreshold(MoodType.Fear));
        thresholdMoodValues.Add(MoodType.Disgust, MoodSwitchThreshold(MoodType.Disgust));
        moodActivation = MoodType.Joy;
        listenerChange = false;
        var children = GetComponentsInChildren<Transform>();

        var container = children.Where(child => child.tag == "ContainerUI").ToArray();
        Debug.Log(container[0].name);
        if (container != null)
            containerUI = container[0].gameObject;

        var baloons = children.Where(child => child.tag == "Baloon").ToArray();
        if (baloons != null)
            baloon = baloons[0].gameObject;
    }

    private void Update()
    {
        containerUI.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    
        if (listenerChange)
        {
            if (!lockMood)
            {
                var moodRef = DisplayController.instance.moodDict[moodActivation];
                currentMoodValues[moodActivation] += incrementMood;
                Debug.Log(this.gameObject.name + " - increase - " + currentMoodValues[moodActivation] + " - threshold - " + thresholdMoodValues[moodActivation]);
                if (DisplayController.instance.npc == transform.gameObject)
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
                    if (DisplayController.instance.npc == this)
                    {
                        DisplayController.instance.currentMoodDisplay.GetComponent<Image>().sprite = moodRef.sprite;
                        DisplayController.instance.currentMoodDisplay.GetComponent<Image>().color = moodRef.color;
                    }
                    
                }
                listenerChange = false;
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
                var moodRef = DisplayController.instance.moodDict[moodToCooldown];
                if (DisplayController.instance.npc == transform.gameObject) { }
                    moodRef.bar.value -= incrementMood;
                if (currentMoodValues[moodToCooldown] < thresholdMoodValues[moodToCooldown])
                {
                    mood = MoodType.Neutral;
                    GetComponent<HogwartsStudent>().durationActionInfluence = 1f;
                    GetComponent<HogwartsStudent>().successActionInfluence = 1f;
                    baloon.transform.GetComponent<Image>().color = new Color(255, 255, 255, 0);
                    baloon.transform.GetComponent<Image>().sprite = null;
                    if (DisplayController.instance.npc == this)
                    {
                        DisplayController.instance.currentMoodDisplay.GetComponent<Image>().sprite = DisplayController.instance.moodDict[MoodType.Neutral].sprite;
                        DisplayController.instance.currentMoodDisplay.GetComponent<Image>().color = DisplayController.instance.moodDict[MoodType.Neutral].color;
                    }
                }
            }
        }
    }

    //Calculate value of each mood threshold depending on the personality model OCEAN 
    public float MoodSwitchThreshold(MoodType mood)
    {
        //default value with no personality
        float threshold = 5f;
        switch (mood)
        {
            case MoodType.Joy:
                threshold -= (neuroticism + extraversion);
                break;
            case MoodType.Sad:
                threshold -= neuroticism;
                break;
            case MoodType.Angry:
                threshold -= (neuroticism - agreeableness);
                break;
            case MoodType.Fear:
                threshold -= neuroticism;
                break;
            case MoodType.Disgust:
                threshold -= (neuroticism - agreeableness);
                break;
            default:
                break;
        }
        return threshold;
    }

    public float OpennessCostManipulation(float cost, float increment)
    {
       //change this depending on what you need
       cost += increment / 2 * openness;
       return cost;
    }

    public float ExtraversionCostManipulation(float cost, float increment)
    {
        //change this depending on what you need
        cost -= increment / 2 * extraversion;
        return cost;
    }

    public float AgreeablenessCostManipulation(float cost, float increment)
    {
        //change this depending on what you need
        cost += increment / 2 * agreeableness;
        return cost;
    }

    //Used to trigger FEAR mood on High Agreeableness NPC
    public bool CheckConsentPeopleAround(List<GameObject> consentNPCs)
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, new Vector2(2, 2), 0f, LayerMask.GetMask("NPC"));
        if (agreeableness > 0)
        {
            foreach (GameObject consent in consentNPCs)
                if (hit.name == consent.name)
                {
                    DisplayController.instance.ChangeMood(MoodType.Fear, 5);
                    return true;
                }
            
        }
        return false;
    }

    private void OnMouseOver()
    {
        DisplayController.instance.cursor.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/ui_cursor_investigate");    
    }
    private void OnMouseExit()
    {
        DisplayController.instance.cursor.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/ui_cursor");
    }





}
