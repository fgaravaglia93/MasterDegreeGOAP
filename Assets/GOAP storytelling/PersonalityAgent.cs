using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalityAgent : Agent
{

    int extraversion;

    void Start()
    {
        extraversion = GetComponent<BigFivePersonality>().extraversion;
    }

    public override void IdleState(FSM fsm, GameObject agent)
    {
        
        //manage extraversion factor on available actions
        foreach (PersonalityAction action in m_availableActions)
            if (action.interactFlag)
            {
                //action.cost = action.cost - (action.cost/2*extraversion);
                action.cost -= action.cost / 2 * extraversion;
                print(action.cost + " - " + action.nameAction);
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
        if (m_eqsEventOccurred)
        {
            DisplayController.instance.ShowOnConsolePlan("Event occurred. Recalculate plan");
        }
    }

    public override void PerformActionState(FSM fsm, GameObject agent)
    {

        base.PerformActionState(fsm, agent);

        GoapAction action = m_currentActions.Peek();
        if (action.IsDone())
        {

            if (action.CalculateSuccess())
                m_currentActions.Dequeue();
            else
            {
                //DisplayController.instance.ShowOnConsoleAction("Action failed, repeat");

                action.OnReset();
            }


        }

    }

    string PrintPlanActions()
    {
        string printedPlan="Plan Found:";
        int i = 1;

        foreach (PersonalityAction action in plan)
        {
            printedPlan += ("\n)"+i+" "+(action.console));
            i++;
        }
        print(printedPlan);
        return printedPlan;
    }





}
