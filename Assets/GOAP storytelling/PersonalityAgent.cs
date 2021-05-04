using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PersonalityAgent : Agent
{

    public bool planning;
    public bool interaction = false;
    public bool displayed = false;
    
    int extraversion;
    int agreeableness;
    bool firstTime = true;
    bool firsHitAction;

    bool traitChange;
    
    //used to be shown on overlay UI
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
            /*foreach (PersonalityAction action in m_availableActions)
            {
                if (action.interactFlag)
                {
                    action.cost = GetComponent<BigFivePersonality>().ExtraversionCostManipulation(action.cost, action.initialCost);
                    //print(action.cost + " - " + action.nameAction);
                }

                if(action.consentNPCs.Count > 0)
                {
                    action.cost = GetComponent<BigFivePersonality>().AgreeablenessCostManipulation(action.cost, action.initialCost);

                }
            }*/
            firstTime = false;
        }
        
        base.IdleState(fsm, agent);

        if(plan != null)
        {
            planListText = PrintPlanActions();
            DisplayController.instance.ShowOnConsolePlan(planListText);
        }
        else
        {
            planListText = "Plan not Found";
            DisplayController.instance.ShowOnConsolePlan(planListText);
        }
    }

    public override void MoveToState(FSM fsm, GameObject agent)
    {
        base.MoveToState(fsm, agent);
        if (m_eqsEventOccurred!=null)
        {
            planListText = "Event occurred. Recalculate plan";
            DisplayController.instance.ShowOnConsolePlan(planListText);
        }
    }

    /*public override void PerformActionState(FSM fsm, GameObject agent)
    {
        PersonalityAction actionSucc = null;

        if (firsHitAction && plan.Peek()!=null)
        {
            actionSucc = (PersonalityAction)plan.Peek();
            string actionName = actionSucc.console;
           
        }

        base.PerformActionState(fsm, agent);

        if (actionSucc.CalculateSuccess())
        {
            firsHitAction = true;
           // DisplayController.instance.ShowOnConsoleAction("Done", new Color(0, 255, 0));
        }
        else
        {
            firsHitAction = false;
            DisplayController.instance.ShowOnConsoleAction("Action failed, repeat", new Color(255, 0, 0));
        }


    }*/

    string PrintPlanActions()
    {
        string printedPlan="Plan Found:";
        int i = 1;

        foreach (PersonalityAction action in plan)
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
