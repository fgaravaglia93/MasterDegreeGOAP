using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutInThePot : PersonalityAction
{
  
    private Transform targetPut;

    public PutInThePot()
    {
        AddPrecondition("preparedIngredient", true);
        AddPrecondition("potIsFree", true);
        AddEffect("potionIsReady", true);

        console = console + "Put in the Pot";
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        targetPut = GameObject.FindGameObjectsWithTag("Pot")[0].transform;
        target = targetPut.gameObject;
        return target != null;
    }


}