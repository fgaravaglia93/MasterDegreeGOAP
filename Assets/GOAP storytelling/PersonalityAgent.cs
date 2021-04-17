using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PersonalityAgent : Agent
{

    int extraversion;
    int agreeableness;
    bool firstTime = true;
    bool firsHitAction;

    bool traitChange;

    void Start()
    {
        firsHitAction = true;
        extraversion = GetComponent<BigFivePersonality>().extraversion;
        agreeableness = GetComponent<BigFivePersonality>().agreeableness;
    }

    
    public override void IdleState(FSM fsm, GameObject agent)
    {
        if (firstTime)
        {
            //manage extraversion factor on available actions
            foreach (PersonalityAction action in m_availableActions)
            {
                if (action.interactFlag)
                {
                    //action.cost = action.cost - (action.cost/2*extraversion);
                    action.cost -= action.cost / 2 * extraversion;
                    //print(action.cost + " - " + action.nameAction);
                }
            }
            firstTime = false;
        }
        

        base.IdleState(fsm, agent);

        if(plan != null)
        {
            DisplayController.instance.ShowOnConsolePlan(PrintPlanActions());
        }
        else
        {
            DisplayController.instance.ShowOnConsolePlan("Plan not Found");
        }
       

    }

    public override void MoveToState(FSM fsm, GameObject agent)
    {
        base.MoveToState(fsm, agent);
        if (m_eqsEventOccurred!=null)
        {
            DisplayController.instance.ShowOnConsolePlan("Event occurred. Recalculate plan");
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
            printedPlan += ("\n"+i+") "+(action.console));
            i++;
        }
        print(printedPlan);
        return printedPlan;
    }

    





}
