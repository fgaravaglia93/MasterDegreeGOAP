using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeIngredient : GoapAction
{

    private bool ingredientTaken = false;
    private float startTime = 0;
    public float duration = 2;
    private Transform targetTake;

    public TakeIngredient()
    {
        AddEffect("hasIngredient", true);
    }

    public override bool Perform(GameObject agent)
    {
        if (startTime == 0)
        {
            startTime = Time.time;
            DisplayController.instance.ShowOnConsole("Action: Take ingredient");
            GetComponentInChildren<CompletionBar>().gameObject.GetComponent<Canvas>().enabled = true;
            //GetComponentInChildren<CompletionBar>().gameObject.transform.position = new Vector3(transform.position.x,transform.position.y+1f,transform.position.z);
            GetComponentInChildren<CompletionBar>().StartTaskBar(GetComponent<Person>().durationChange * duration);
            DisplayController.instance.ShowOnConsole("Action: Prepare ingredient");

        }
        if (Time.time - startTime > duration*GetComponent<Person>().durationChange)
        {
            ingredientTaken = true;
            Debug.Log("Take Ingredient: Ingredient taken");
            GetComponentInChildren<CompletionBar>().gameObject.GetComponent<Canvas>().enabled = false;
        }

        return true;
    }

    public override void OnReset()
    {
        ingredientTaken = false;
        // workstation = null;
        startTime = 0;
    }

    public override bool IsDone()
    {
        return ingredientTaken;
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
