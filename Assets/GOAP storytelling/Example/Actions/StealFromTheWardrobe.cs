using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealFromTheWardrobe : PersonalityAction
{
    
    private Transform targetSteal;
    

    public StealFromTheWardrobe()
    {
        AddEffect("potionIsReady", true);

        console = console + "Take ingredient";
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