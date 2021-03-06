using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GoalsList
{
    public static Goal MakePotion() {
        Goal goal = new Goal("MakePotion");
        goal.AddState(new KeyValuePair<string, bool>("potionIsDelivered", true));
        return goal;
    }


    public static void Test() {

        Goal goal1 = new Goal("MakePotion");
        goal1.AddState(new KeyValuePair<string, bool>("potionIsDelivered", true));

        Debug.Log("TEST  " + goal1.m_nameGoal);
    }

}
