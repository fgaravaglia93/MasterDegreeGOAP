using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Student : HogwartsStudent
{
    /**
	 * Our only goal will ever be to chop logs.
	 * The ChopFirewoodAction will be able to fulfill this goal.
	 */
    public override Goal CreateGoalStates()
    {
        /*HashSet<KeyValuePair<string,object>> goalTargets = new HashSet<KeyValuePair<string,object>> ();

		goalTargets.Add(new KeyValuePair<string, object>("collectFirewood", true ));

		return new Goal(goalTargets);*/
        return GoalsList.MakePotion();
    }

    public override GoalStack GetAllGoals()
    {
        GoalStack goalStack = new GoalStack();

        //add all goals to return
        goalStack.Push(CreateGoalStates());

        return goalStack;
    }
}
