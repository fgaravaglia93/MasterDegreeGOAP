using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mood
{
    public MoodType name;
    public float threshold;
    public Sprite sprite;
    public Color color;
    public Slider bar;
    public RectTransform placeHolder;
    public float durationChange;
    public float successChange;

    //for neutral mood
    public Mood(string spritePath)
    {
        name = MoodType.Neutral;
        sprite = Resources.Load<Sprite>(spritePath);
        color = new Color(255, 255, 255);
    }

    public Mood(MoodType moodType, string spritePath, Slider bar, float threshold)
    {
        name = moodType;
        this.threshold = threshold;
        sprite = Resources.Load<Sprite>(spritePath);
        this.bar = bar;
        placeHolder = bar.transform.GetChild(2).GetComponent<RectTransform>();
        placeHolder.localPosition = placeHolder.localPosition + (new Vector3(threshold * 9f, 0f, 0f));


        bar.minValue = 0f;
        bar.maxValue = 7f;

        switch (moodType)
        {
            case MoodType.Joy:
                color = new Color(1f, 1f, 0);
                durationChange = 0.5f;
                successChange = 1f;
                break;
            case MoodType.Sad:
                color = new Color(0, 0, 1f);
                durationChange = 2f;
                successChange = 1f;
                break;
            case MoodType.Angry:
                color = new Color(1f, 0, 0);
                durationChange = 0.5f;
                successChange = 0.5f;
                break;
            case MoodType.Fear:
                color = new Color(1f, 0, 1f);
                durationChange = 1f;
                successChange = 0.5f;
                break;
            case MoodType.Disgust:
                color = new Color(0, 1f, 0);
                durationChange = 0f;
                successChange = 0f;
                break;
            default:
                break;

        }
    }

}



