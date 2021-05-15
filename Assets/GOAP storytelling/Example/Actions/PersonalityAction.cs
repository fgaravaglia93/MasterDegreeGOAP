using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalityAction : GoapAction
{
    private bool performed = false;
    private float startTime = 0;
    public float duration;
    public float initialCost;
    

    [HideInInspector]
    public string console = "Action: ";
    
    //default 1f = 100% of success
    public float percentageSuccess;
    //default not interact  action
    public bool interactAction = false;
    public List<GameObject> consentNPCs;

    private void Start()
    {
        initialCost = cost;
    }
    public override bool Perform(GameObject agent)
    {
        if (startTime == 0)
        {
            startTime = Time.time;
            //DisplayController.instance.ShowOnConsole(console);
            GetComponentInChildren<CompletionBar>().transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            //GetComponent<BigFivePersonality>().containerUI.GetComponentInChildren<Slider>().gameObject.SetActive(true);
            GetComponentInChildren<CompletionBar>().StartTaskBar(GetComponent<HogwartsStudent>().durationActionInfluence * duration);
            DisplayController.instance.lockMood = true;
        }
        if (Time.time - startTime > duration * GetComponent<HogwartsStudent>().durationActionInfluence)
        {
            performed = true;
            //GetComponentInChildren<CompletionBar>().transform.GetChild(2).gameObject.SetActive(true);
            GetComponentInChildren<CompletionBar>().transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            //GetComponent<BigFivePersonality>().containerUI.GetComponentInChildren<Slider>().gameObject.SetActive(false);
        }
        return true;
    }

    public override void OnReset()
    {
        performed = false;
        startTime = 0;
    }

   
    public override bool IsDone()
    {
        return performed;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        return target != null;
    }


    public override bool CalculateSuccess()
    {
        float percentage = percentageSuccess * GetComponent<HogwartsStudent>().successActionInfluence;
        float success = Random.Range(0.0f,1.0f);

        if (success <= percentage)
        {
            // Debug.Log("%: "+success+" <= "+percentage+"\nAction done");

            //release lock on emotion
            DisplayController.instance.lockMood = false;

            //manipulate action cost based on Openness factor
            cost = GetComponent<BigFivePersonality>().OpennessCostManipulation(cost,initialCost);

            //agreeaableness factor > 0 : if the CONSENT is around enter in fear mood 
            //GetComponent<BigFivePersonality>().CheckConsentPeopleAround(consentNPCs);

            return true;
          
        }
        //Debug.Log("%: " + success + " <= " + percentage + "\nAction done");
        return false;
    }

    IEnumerator DoneMessage()
    {
        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<CompletionBar>().transform.GetChild(2).gameObject.SetActive(false);
    }

}
