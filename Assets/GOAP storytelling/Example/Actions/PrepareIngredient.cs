using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrepareIngredient : Moody5Action
{

    private Transform targetPrepare;

    public PrepareIngredient()
    {
        AddPrecondition("recipeIsKnown", true);
        AddPrecondition("hasIngredient", true);
        AddPrecondition("workstationIsFree", true);
        AddEffect("preparedIngredient", true);
        console = console + "Prepare ingredient";
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    //Prende il bancone più vicino dove andare a preparare la pozione (pathfind), a me non serve
    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        targetPrepare = GameObject.FindGameObjectsWithTag("Workstation")[0].transform;
        target = targetPrepare.gameObject;
        return target != null;
    }
}
