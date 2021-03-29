using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalityAction : GoapAction
{
    private bool performed = false;
    private float startTime = 0;
    public float duration;


    [HideInInspector]
    public string console = "Action: ";
    
    //default 100% of success
    public float percentageSuccess = 1f;
    //default not interact  action
    public bool interactFlag = false;
    public List<GameObject> consentNPCs;

    public override bool Perform(GameObject agent)
    {
        if (startTime == 0)
        {
            startTime = Time.time;
           // DisplayController.instance.ShowOnConsole(console);
            GetComponentInChildren<CompletionBar>().transform.GetChild(0).gameObject.SetActive(true);
            //GetComponentInChildren<CompletionBar>().gameObject.transform.position = new Vector3(transform.position.x,transform.position.y+1f,transform.position.z);
            GetComponentInChildren<CompletionBar>().StartTaskBar(GetComponent<HogwartsStudent>().durationActionInfluence * duration);
        }
        if (Time.time - startTime > duration * GetComponent<HogwartsStudent>().durationActionInfluence)
        {
            performed = true;
            GetComponentInChildren<CompletionBar>().transform.GetChild(2).gameObject.SetActive(true);
            StartCoroutine("DoneMessage");
            GetComponentInChildren<CompletionBar>().transform.GetChild(0).gameObject.SetActive(false);
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
            Debug.Log("%: "+success+" <= "+percentage+"\nAction done");
            return true;
        }
        return false;
    }

    IEnumerator DoneMessage()
    {
        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<CompletionBar>().transform.GetChild(2).gameObject.SetActive(false);
    }

}
