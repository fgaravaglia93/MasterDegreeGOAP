using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AskForHelp : Moody5Action
{

    private Transform targetTake;

    public AskForHelp()
    {
        AddEffect("recipeIsKnown", true);
        console = console + "Ask for help";
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        targetTake = GameObject.FindGameObjectsWithTag("Helper")[0].transform;
        target = targetTake.gameObject;
        return target != null;
    }
}


