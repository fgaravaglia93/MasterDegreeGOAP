using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverPotion : Moody5Action
{

    private Transform targetDeliver;

    public DeliverPotion()
    {
        AddPrecondition("potionIsReady", true);
        AddEffect("potionIsDelivered", true);

        console = console + "Deliver potion";
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
