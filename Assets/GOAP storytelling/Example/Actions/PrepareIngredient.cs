using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrepareIngredient : GoapAction
{

    private bool prepared = false;
    //private WorkStation workstation; // where we forge tools
    private float startTime = 0;
    public float duration = 5; // seconds
    private Transform targetPrepare;

    public PrepareIngredient()
    {
        AddPrecondition("hasIngredient", true);
        AddPrecondition("workstationIsFree", true);
        AddEffect("preparedIngredient", true);
    }

    public override void OnReset()
    {
        prepared = false;
       // workstation = null;
        startTime = 0;
    }

    public override bool Perform(GameObject agent)
    {
        if (startTime == 0)
        {
            startTime = Time.time;
            GetComponentInChildren<CompletionBar>().gameObject.GetComponent<Canvas>().enabled = true;
            //GetComponentInChildren<CompletionBar>().gameObject.transform.position = new Vector3(transform.position.x,transform.position.y+1f,transform.position.z);
            GetComponentInChildren<CompletionBar>().StartTaskBar(GetComponent<Person>().durationChange * duration);
            DisplayController.instance.ShowOnConsole("Action: Prepare ingredient");
        }

        if (Time.time - startTime > GetComponent<Person>().durationChange*duration)
        {
            // finished forging a tool
            prepared = true;
            Debug.Log("Prepare Ingredient: Ingredient prepared");
            GetComponentInChildren<CompletionBar>().gameObject.GetComponent<Canvas>().enabled = false;
        }
        return true;
    }

    public override bool IsDone()
    {
        return prepared;
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
