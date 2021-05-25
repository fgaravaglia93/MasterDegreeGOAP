using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MoodType { Neutral, Joy, Sadness, Angry, Fear, Disgust }

public class BigFivePersonality : MonoBehaviour
{
   
    [Space(10)]
    [Header("Big Five personality model")]
    [Tooltip("Openness to Experience.\nAgent preference on new actions")]
    [Range(-1, 1)]
    public int openness;
    [Range(-1, 1)]
    public int conscientiousness;
    [Range(-1, 1)]
    public int extraversion;
    [Range(-1, 1)]
    public int agreeableness;
    [Range(-1, 1)]
    public int neuroticism;

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
    public bool CheckConsentPeopleAround(List<GameObject> consentNPCs, Transform transform)
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, new Vector2(2, 2), 0f, LayerMask.GetMask("NPC"));
        if (agreeableness > 0)
        {
            foreach (GameObject consent in consentNPCs)
                if (hit.name == consent.name)
                {
                    DisplayManager.instance.ChangeMood(MoodType.Fear, 5);
                    return true;
                }
            
        }
        return false;
    }

   





}
