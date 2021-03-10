using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeIngredient : PersonalityAction
{
  
    private Transform targetTake;

    public TakeIngredient()
    {
        AddEffect("hasIngredient", true);
        console =  console + "Take ingredient";
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        targetTake = GameObject.FindGameObjectsWithTag("Fridge")[0].transform;
        target = targetTake.gameObject;
        return target != null;
    }
}
