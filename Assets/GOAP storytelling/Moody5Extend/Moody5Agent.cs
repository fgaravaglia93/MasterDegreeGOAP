﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem.DataContainers;


public class Moody5Agent : Agent
{

    public bool planning;
    public bool interaction = false;
    public bool displayed = false;
    
    int extraversion;
    int agreeableness;
    bool firstTime = true;
    bool firsHitAction;

    bool traitChange;
    public DialogueContainer goapInteraction;
    
    
    //used on overlay UI
    /*[HideInInspector]
    public string planListText;
    [HideInInspector]
    public string actionText;
    [HideInInspector]
    public string goalText;*/
    

    void Start()
    {
        firsHitAction = true;
        extraversion = GetComponent<BigFivePersonality>().extraversion;
        agreeableness = GetComponent<BigFivePersonality>().agreeableness;
    }

    public override void Update()
    {
        if(planning && !interaction)
            base.Update();

    }

    public override void IdleState(FSM fsm, GameObject agent)
    {
        if (firstTime)
        {
            //manage extraversion factor on available actions
            foreach (Moody5Action action in m_availableActions)
            {
                if (action.interactAction)
                {
                    action.cost = GetComponent<BigFivePersonality>().ExtraversionCostManipulation(action.cost, action.bigFiveWeight);
                    //print(action.cost + " - " + action.nameAction);
                }

                //Manage agreeableness factor by searching for consent NPCs for each action
                if(action.consentNPCs.Count > 0)
                {
                    action.cost = GetComponent<BigFivePersonality>().AgreeablenessCostManipulation(action.cost, action.initialCost);

                }
            }
            firstTime = false;
        }
        
        base.IdleState(fsm, agent);

        if(plan != null)
        {
            planListText = PrintPlanActions();
            DisplayManager.instance.ShowOnConsolePlan(planListText);
        }
        else
        {
            planListText = "Plan not Found";
            DisplayManager.instance.ShowOnConsolePlan(planListText);
        }
    }

    public override void MoveToState(FSM fsm, GameObject agent)
    {
        base.MoveToState(fsm, agent);
        if (m_eqsEventOccurred!=null)
        {
            planListText = "Event occurred. Recalculate plan";
            DisplayManager.instance.ShowOnConsolePlan(planListText);
            StartCoroutine("TraitAdHocHighlight");
        }
    }

    public IEnumerable TraitAdHocHighlight()
    {
        Debug.Log(GetComponent<MoodController>().textTrait);
        yield return new WaitForSeconds(1f);
        DisplayManager.instance.ShowOnConsoleTraitAdHoc(GetComponent<MoodController>().textTrait, GetComponent<MoodController>().colorTrait);

    }

    //Used to print the list of action to be displayed on the investigative UI
    string PrintPlanActions()
    {
        string printedPlan="Plan Found:";
        int i = 1;

        foreach (Moody5Action action in plan)
        {
            if (action.nameAction == "Steal")
                action.console = "Steal from the wardrobe";
            printedPlan += ("\n"+i+") "+(action.console));
            i++;
        }
        print(printedPlan);
        return printedPlan;
    }

    





}
