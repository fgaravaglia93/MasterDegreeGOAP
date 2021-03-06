using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutInThePot : GoapAction
{
    private bool potionReady = false;
    private float startTime = 0;
    public float duration = 2;
    private Transform targetPut;

    public PutInThePot()
    {
        AddPrecondition("preparedIngredient", true);
        AddPrecondition("potIsFree", true);
        AddEffect("potionIsReady", true);
    }

    public override bool Perform(GameObject agent)
    {
        if (startTime == 0)
        {
            startTime = Time.time;
            DisplayController.instance.ShowOnConsole("Action: Put in the pot");
        }

        if (Time.time - startTime > duration)
        {
            potionReady = true;
            Debug.Log("Put in the Pot: Potion is ready");
        }

        return true;
    }

    public override void OnReset()
    {
        potionReady = false;
        // workstation = null;
        startTime = 0;
    }

    public override bool IsDone()
    {
        return potionReady;
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