using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoodType { Neutral, Joy, Sad, Angry, Fear, Disgust }

public class PersonalityAgent : Agent
{

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


    public MoodType mood = MoodType.Neutral;

    

    public override void IdleState(FSM fsm, GameObject agent)
    {

        foreach (PersonalityAction action in m_availableActions)
            if (action.interactFlag)
            {
                //action.cost = action.cost - (action.cost/2*extraversion);
                action.cost -= action.cost / 2 * extraversion;
                print(action.cost + " - " + action.nameAction);
            }

        base.IdleState(fsm, agent);
        
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
   
}
