using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
//-Fra aggiunto da me per compilare
//vecchio era già compilato
public enum EventTriggerType { All, AtLeastOne, None }

[Serializable]
public class TraitData
{
	public string name;
	public float weight = 1f;
	public float radius;

	[Space(10)]
	public InfluenceType m_influenceType;

	[Space(10)]
	public EventTriggerType m_eventTriggerType;
	//[Range(0,2)]
	//public float m_triggerThreshold;

	[Space(10)]
	public Trait trait;

	[Space(10)]
	public List<TestsRuntimeVariables> m_testsRuntimeVariables;

	private float m_result;
	[HideInInspector]
	public float[] m_testResults;
	//[HideInInspector]
	public List<bool> m_outcomeResults;
	//[HideInInspector]
	public bool m_occurred = false;

    private GameObject[] oldPathArray=new GameObject[0];

	public void Init(EQSAgent EQSAgent) {
		name = trait.name;
		m_testResults = new float[trait.EQSTests.Count];
		m_occurred = false;
		/*Type type = Type.GetType(m_influenceType.goalType);
		if(type != null) {
			m_influenceType.goal = (Goal)Activator.CreateInstance(type);
		}*/
		List<TestsRuntimeVariables>  m_oldTestsRuntimeVariables = new List<TestsRuntimeVariables>();
		foreach(TestsRuntimeVariables runtimeVariable in m_testsRuntimeVariables) {
			m_oldTestsRuntimeVariables.Add(runtimeVariable);
		}

		m_testsRuntimeVariables = new List<TestsRuntimeVariables>();
		m_outcomeResults = new List<bool>();

		List<object> values = new List<object>();
		if(trait.EQSTests.Count == m_oldTestsRuntimeVariables.Count) {
			for(int i = 0; i < m_oldTestsRuntimeVariables.Count; i++) {
				values = new List<object>();
				foreach(var thisVar in m_oldTestsRuntimeVariables[i].GetType().GetFields()) {
					values.Add(thisVar.GetValue(m_oldTestsRuntimeVariables[i]));
				}
				m_testsRuntimeVariables.Add(new TestsRuntimeVariables(trait.EQSTests[i].name, trait.EQSTests[i].GetType(), values));
				m_outcomeResults.Add(false);
			}
		}
		else {
			for(int i = 0; i < trait.EQSTests.Count; i++) {
				m_testsRuntimeVariables.Add(new TestsRuntimeVariables(trait.EQSTests[i].name, trait.EQSTests[i].GetType(), null));
				m_outcomeResults.Add(false);
			}
		}
	}

	public void RunTraitTest(Personality personality) {
		trait.RunTestsForTrait(personality, this);
		NormalizeTestResults();
	}

	public void NormalizeTestResults() {
		//Normalize Test Result Values Between 0 and 1
		for(int i = 0; i < m_testResults.Length; i++) {
			if(!trait.EQSTests[i].isActive || !(m_outcomeResults[i]))
				continue;
			if(radius != 0f) {
				if(trait.EQSTests[i].maxValue == 0)
					m_testResults[i] = m_testResults[i] / radius;
				else
					m_testResults[i] = m_testResults[i] / trait.EQSTests[i].maxValue;
			}
		}
	}

	public float SumValuesOfTests() {

		float weightedScore = 0f;
		float boolResult = 0f;
		float numResult = 0f;

		for(int i = 0; i < m_testResults.Length; i++) {

			if(!trait.EQSTests[i].isActive || !(m_outcomeResults[i]))
				continue;

			switch(trait.EQSTests[i].scoringEquation) {
				case EQSTestScoringEquation.Linear:
					weightedScore = trait.EQSTests[i].weight * m_testResults[i];
					break;
				case EQSTestScoringEquation.InverseLinear:
					weightedScore = trait.EQSTests[i].weight * (1.0f - m_testResults[i]);
					break;
				case EQSTestScoringEquation.Square:
					weightedScore = trait.EQSTests[i].weight * (m_testResults[i] * m_testResults[i]);
					break;
				case EQSTestScoringEquation.HalfSine:
					weightedScore = trait.EQSTests[i].weight * Mathf.Sin(Mathf.PI * m_testResults[i]);
					break;
				case EQSTestScoringEquation.InverseHalfSine:
					weightedScore = trait.EQSTests[i].weight * -Mathf.Sin(Mathf.PI * m_testResults[i]);
					break;
				case EQSTestScoringEquation.HalfSineSquared:
					weightedScore = trait.EQSTests[i].weight * Mathf.Sin(Mathf.PI * m_testResults[i]) * Mathf.Sin(Mathf.PI * m_testResults[i]);
					break;
				case EQSTestScoringEquation.InverseHalfSineSquared:
					weightedScore = trait.EQSTests[i].weight * -Mathf.Sin(Mathf.PI * m_testResults[i]) * Mathf.Sin(Mathf.PI * m_testResults[i]);
					break;
				case EQSTestScoringEquation.Sigmoid:
					weightedScore = trait.EQSTests[i].weight * (((float)Math.Tanh(4.0f * (m_testResults[i] - 0.5f)) + 1.0f) * 0.5f);
					break;
				case EQSTestScoringEquation.InverseSigmoid:
					weightedScore = trait.EQSTests[i].weight * (1.0f - (((float)Math.Tanh(4.0f * (m_testResults[i] - 0.5f)) + 1.0f) * 0.5f));
					break;
				default:
					break;
			}

			if(trait.EQSTests[i].resultType == TestResultType.Boolean)
				boolResult += Mathf.Abs(weightedScore * weight);
			else if(trait.EQSTests[i].resultType == TestResultType.Numerical)
				numResult += Mathf.Abs(weightedScore * weight);
		}
		if(boolResult > 1f)
			boolResult = 1f;
		m_result = boolResult + numResult;
		return m_result;
	}

	public void OnEventCostRisen(EQSAgent agent) {
		if(m_influenceType.responseType == ResponseType.Add) {
			foreach(GoapAction action in agent.m_owner.m_availableActions) {
				if(action.GetType().ToString().Equals(m_influenceType.actionType)) {
					if(!m_influenceType.costModified) {
						action.eqsCost += m_influenceType.costIncrement * m_result;
						m_influenceType.costModified = true;
					}
					break;
				}
			}
		}
		else if(m_influenceType.responseType == ResponseType.Remove) {
			foreach(GoapAction action in agent.m_owner.m_availableActions) {
				if(action.GetType().ToString().Equals(m_influenceType.actionType)) {
					if(!m_influenceType.costModified) {
						action.eqsCost -= m_influenceType.costIncrement * m_result;
						m_influenceType.costModified = true;
					}
					break;
				}
			}
		}
		m_occurred = true;
	}

	public void OnEventActionRisen(EQSAgent agent) {
		if(m_influenceType.responseType == ResponseType.Add) {
			foreach(GoapAction action in agent.m_owner.m_availableActions) {
				if(action.GetType().ToString().Equals(m_influenceType.actionType)) {
					agent.m_owner.AddAction(action);
					break;
				}
			}
		}
		else if(m_influenceType.responseType == ResponseType.Remove) {
			foreach(GoapAction action in agent.m_owner.m_availableActions) {
				if(action.GetType().ToString().Equals(m_influenceType.actionType)) {
					agent.m_owner.RemoveAction(action);
					break;
				}
			}
		}
		m_occurred = true;
	}

	public void OnEventGoalRisen(EQSAgent agent) {
		if(m_influenceType.responseType == ResponseType.Add) {
			//agent.m_owner.m_goalStack.Push(m_influenceType.goal);
            MethodInfo methodInfo = typeof(GoalsList).GetMethod(m_influenceType.goal);
            agent.m_owner.m_goalStack.Push(methodInfo.Invoke(this, null) as Goal);
            //foreach(Goal goal in agent.m_owner.m_goalStack.GetGoals()) {
            //	agent.m_owner.m_goalStack.Push(m_influenceType.goal);
            //	return;
            //}
        }
		else if(m_influenceType.responseType == ResponseType.Remove) {
            MethodInfo methodInfo = typeof(GoalsList).GetMethod(m_influenceType.goal);
            agent.m_owner.m_goalStack.Remove(methodInfo.Invoke(this, null) as Goal);
			//foreach(Goal goal in agent.m_owner.m_goalStack.GetGoals()) {
			//	if(goal.GetType().ToString().Equals(m_influenceType.goalType)) {
			//		agent.m_owner.m_goalStack.Remove(goal);
			//		return;
			//	}
			//}
		}
		m_occurred = true;
	}

    public void OnEventPathRisen(EQSAgent agent) {
        GameObject[] hexTiles = GameObject.FindGameObjectsWithTag(name);
/*-Fra      
	  //-Fra Labourer labourer = m_influenceType.labourer;

        //Collider[] colliders = Physics.OverlapSphere(agent.m_owner.transform.position, radius, LayerMask.GetMask("Tests") + LayerMask.GetMask("WalkableLayer") + LayerMask.GetMask("BlockingLayer"));

        if(m_influenceType.responseType == ResponseType.Add) {
            foreach(GameObject tile in hexTiles) {
                if(tile.CompareTag(name) && Vector3.Distance(tile.transform.position, agent.m_owner.transform.position) <= radius+25) {
                    if(!labourer.tilesEQSCosts.ContainsKey(tile.GetComponent<HexTile>())) {
                        labourer.tilesEQSCosts.Add(tile.GetComponent<HexTile>(), (int)m_influenceType.costIncrement);
                    }
                }
            }
            /vecchio*for(int i = 0; i < colliders.Length; i++) {
                if(colliders[i].CompareTag(name)) {
                    if(!labourer.tilesEQSCosts.ContainsKey(colliders[i].GetComponent<HexTile>())) {
                        labourer.tilesEQSCosts.Add(colliders[i].GetComponent<HexTile>(), (int)m_influenceType.costIncrement);
                    }
                }
            }*vecchio/
        }
       else if(m_influenceType.responseType == ResponseType.Remove) {
            foreach(GameObject tile in hexTiles) {
                if(tile.CompareTag(name) && Vector3.Distance(tile.transform.position, agent.m_owner.transform.position) < radius) {
                    if(!labourer.tilesEQSCosts.ContainsKey(tile.GetComponent<HexTile>())) {
                        labourer.tilesEQSCosts.Add(tile.GetComponent<HexTile>(), -(int)m_influenceType.costIncrement);
                    }
                }
            }
			
			-Fra */
            /*vecchiofor(int i = 0; i < colliders.Length; i++) {
                if(colliders[i].CompareTag(name)) {
                    if(!labourer.tilesEQSCosts.ContainsKey(colliders[i].GetComponent<HexTile>())) {
                        labourer.tilesEQSCosts.Add(colliders[i].GetComponent<HexTile>(), -(int)m_influenceType.costIncrement);
                    }
                }
            }*vecchio/
        } -Fra*/
        m_occurred = true;
    }

    /*public void VariableChecker() {
		foreach(Test test in trait.EQSTests) {
			foreach(var thisVar in test.GetType().GetFields()) {
				if(thisVar.DeclaringType != typeof(Test))
					Debug.Log("Var Name:  " + thisVar.Name + "	Type:  " + thisVar.DeclaringType);
			}
		}
	}*/
}
