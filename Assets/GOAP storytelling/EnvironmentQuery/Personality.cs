using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;


[Serializable]
public class Personality
{
	[HideInInspector]
	public Agent m_agent;
	public List<TraitData> m_traitDatas = new List<TraitData>();

    [HideInInspector]
    public bool isGOAP = true;

	//public Dictionary<string, float> oldResults;
	public Dictionary<string, float> results;

	public void Init(Agent agent) {
		//oldResults = new Dictionary<string, float>();
		results = new Dictionary<string, float>();
		m_agent = agent;

		foreach(TraitData traitData in m_traitDatas) {
			//oldResults.Add(traitData.name, 0);
			results.Add(traitData.name, 0);
			traitData.m_testResults = new float[traitData.trait.EQSTests.Count];
		}
	}

    public void Init()
    {
        //oldResults = new Dictionary<string, float>();
        results = new Dictionary<string, float>();
            
        foreach (TraitData traitData in m_traitDatas)
        {
            //oldResults.Add(traitData.name, 0);
            results.Add(traitData.name, 0);
            traitData.m_testResults = new float[traitData.trait.EQSTests.Count];
        }
    }

    public List<TraitData> RunTests() {

		//Save the result of the last test in memory
		/*foreach(string key in results.Keys) {
			oldResults[key] = results[key];
		}*/
		ResetValues();
        List<TraitData> traitDataActivated = new List<TraitData>();
        //Run Each set of tests in the traits
        foreach (TraitData traitData in m_traitDatas) {
			traitData.RunTraitTest(this);
			results[traitData.name] = traitData.SumValuesOfTests();


			if(!CheckIfEventOccurred(traitData)) {
                if(isGOAP)
				    AddEventDelegate(traitData);
                traitDataActivated.Add(traitData);
            }
			else {
				results[traitData.name] = 0f;
			}
			PrintResultsInfo(traitData);
		}
        return traitDataActivated;
	}

	public bool CheckIfEventOccurred(TraitData traitData) {
		if(!traitData.m_occurred && CheckTestOutcomes(traitData)) {
			return false;
		}
		return true;
	}

	public void AddEventDelegate(TraitData traitData) {
		if(traitData.m_influenceType.response.Equals(Response.ActionManipulation))
			m_agent.m_eqsAgent.OnRiseEqsEvents += traitData.OnEventActionRisen;
		else if(traitData.m_influenceType.response.Equals(Response.CostManipulation))
			m_agent.m_eqsAgent.OnRiseEqsEvents += traitData.OnEventCostRisen;
		else if(traitData.m_influenceType.response.Equals(Response.GoalManipulation))
			m_agent.m_eqsAgent.OnRiseEqsEvents += traitData.OnEventGoalRisen;
        else if(traitData.m_influenceType.response.Equals(Response.PathfindingManipulation))
            m_agent.m_eqsAgent.OnRiseEqsEvents += traitData.OnEventPathRisen;
    }

	/// <summary>
	/// Reset the values of the result and variables in traitData
	/// </summary>
	public void ResetValues() {
		foreach(TraitData traitData in m_traitDatas) {
			results[traitData.name] = 0f;
			traitData.m_influenceType.costModified = false;
		}
	}

	/// <summary>
	/// Check the Test Outcome basen on the Event Trigger Type
	/// </summary>
	/// <param name="traitData"></param>
	/// <returns></returns>
	public bool CheckTestOutcomes(TraitData traitData) {
		if(traitData.m_eventTriggerType == EventTriggerType.AtLeastOne) {
			for(int i = 0; i < traitData.trait.EQSTests.Count; i++) {
				if(traitData.m_outcomeResults[i])
					return true;
			}
			return false;
		}
		else if(traitData.m_eventTriggerType == EventTriggerType.All) {
			for(int i = 0; i < traitData.trait.EQSTests.Count; i++) {
				if(!traitData.m_outcomeResults[i])
					return false;
			}
			return true;
		}
        else if(traitData.m_eventTriggerType == EventTriggerType.None) {
            for(int i = 0; i < traitData.trait.EQSTests.Count; i++) {
                if(traitData.m_outcomeResults[i])
                    return false;
            }
            return true;
        }
		return false;
	}

	public void PrintResultsInfo(TraitData traitData) {
		string text = m_agent.gameObject.name +" Sum Score: " + results[traitData.name] + "\n";
		for(int i = 0; i < traitData.m_testResults.Length; i++) {
			//text += "test_" + i + ": " + testResults[traitData.name][i] + "\n";
			text += "test_" + i + ": " + traitData.m_testResults[i] + "\n";
		}

		//Debug.Log(text.Substring(0,text.IndexOf("S")) + text.Substring(text.IndexOf("S")+"S".Length));
		//Debug.Log(text);
	}

    public void AddTrait(TraitData newTraitData)
    {
        Debug.Log("Collisione Trait");
        string pathTrait = "Assets/GOAP storytelling/Example/Traits/" + newTraitData.name + ".asset";
        UnityEngine.Object data = AssetDatabase.LoadAssetAtPath(pathTrait, typeof(Trait));
        newTraitData.trait = (Trait)data;
        newTraitData.m_testsRuntimeVariables = new List<TestsRuntimeVariables>();
        newTraitData.m_outcomeResults = new List<bool>();
        newTraitData.m_testResults = new float[newTraitData.trait.EQSTests.Count];
        
        for (int i = 0; i < newTraitData.trait.EQSTests.Count; i++)
        {
            //Debug.Log(newTraitData.trait.EQSTests[i].GetType());
            newTraitData.m_testsRuntimeVariables.Add(new TestsRuntimeVariables(newTraitData.trait, newTraitData.trait.EQSTests[i].name, newTraitData.trait.EQSTests[i].GetType(), null));
            newTraitData.m_outcomeResults.Add(false);
        }
        bool alreadyAdded = true;
        foreach (TraitData traitData in m_traitDatas)
        {
            if(newTraitData.name == traitData.name)
            {
                alreadyAdded = true;
                break;
            }
        }
        if (alreadyAdded)
        {
            m_traitDatas.Add(newTraitData);
        }
    }

    public void RemoveTrait(string name)
    {
        Debug.Log("Collisione Trait");
        bool found = false;
        int i = 0;
        foreach (TraitData traitData in m_traitDatas)
        {
            if (name == traitData.name)
            {
                found = true;
                
                break;
            }
            i++;
        }
        if(found == true)
            m_traitDatas.RemoveAt(i);

    }

}
