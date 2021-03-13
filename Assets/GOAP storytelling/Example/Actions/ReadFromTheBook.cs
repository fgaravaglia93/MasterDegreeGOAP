using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadFromTheBook : PersonalityAction
{
    private Transform targetTake;

    public ReadFromTheBook()
    {
        AddEffect("recipeIsKnown", true);
        console = console + "Read From TheBook";
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        targetTake = GameObject.FindGameObjectsWithTag("Book")[0].transform;
        target = targetTake.gameObject;
        return target != null;
    }
}
