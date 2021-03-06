using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealFromTheWardrobe : GoapAction
{
    private bool potionReady = false;
    private float startTime = 0;
    public float duration = 1;
    private Transform targetSteal;
    

    public StealFromTheWardrobe()
    {
        AddEffect("potionIsReady", true);
    }

    public override bool Perform(GameObject agent)
    {
        if (startTime == 0)
        {
            startTime = Time.time;
            DisplayController.instance.ShowOnConsole("Action: Steal from the wardrobe");
        }

        if (Time.time - startTime > duration)
        {
            potionReady = true;
            Debug.Log("Action: Steal from the wardrobe: Potion is ready");
        }

        return true;
    }

    public override void OnReset()
    {
        potionReady = false;
        targetSteal = null;
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
        //questo mi serve brutale per gestire il fatto di avere un solo target
        //considera il metodo fatto da Davide con lo script perchè è interessante
        targetSteal = GameObject.FindGameObjectsWithTag("Wardrobe")[0].transform;
        target = targetSteal.gameObject;
        return target != null;
    }


}