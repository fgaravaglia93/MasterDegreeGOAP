using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverPotion : GoapAction
{

    private bool deliver = false;
    //private WorkStation workstation; // where we forge tools
    private float startTime = 0;
    public float duration = 2; // seconds
    private Transform targetDeliver;

    public DeliverPotion()
    {
        AddPrecondition("potionIsReady", true);
        AddEffect("potionIsDelivered", true);
    }

    public override void OnReset()
    {
        deliver = false;
        // workstation = null;
        startTime = 0;
    }

    public override bool Perform(GameObject agent)
    {
        if (startTime == 0)
        {
            startTime = Time.time;
            DisplayController.instance.ShowOnConsole("Action: Deliver potion");
        }

        if (Time.time - startTime > duration)
        {
            // finished forging a tool
            deliver = true;
            Debug.Log("Deliver Potion: Potion Delivered");
            
        }
        return true;
    }

    public override bool IsDone()
    {
        return deliver;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    //Prende il bancone più vicino dove andare a preparare la pozione (pathfind), a me non serve
    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        targetDeliver = GameObject.FindGameObjectsWithTag("Delivery")[0].transform;
        target = targetDeliver.gameObject;
        return target != null;


    }
}
