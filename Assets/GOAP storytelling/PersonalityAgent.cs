using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mood { Neutral, Joy, Sad, Angry, Fear, Disgust }

public class PersonalityAgent : Agent
{

    [Space(10)]
    [Header("BIG 5 personality model")]

    [Range(-1, 1)]
    public int openness;
    [Range(-1, 1)]
    public int consciousness;
    [Range(-1, 1)]
    public int extraversion;
    [Range(-1, 1)]
    public int aggreeableness;
    [Range(-1, 1)]
    public int neuroticism;

    public Mood mood = Mood.Neutral;

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

    public float CalculateSwitchEmotionFactor(int switchFactor)
    {
        return switchFactor -= switchFactor / 2 * neuroticism;
    }
   
}
