using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EQSAgent
{
	public Agent m_owner;
	//public List<Test> EQSTests = new List<Test>();

	public float radius = 5.0f;

	public Personality m_personality;

	//public Dictionary<string,float> InfluenceTypeValues;

	public delegate void RiseEqsEvents(EQSAgent agent);
	public event RiseEqsEvents OnRiseEqsEvents;

	public EQSAgent(Agent owner, Personality personality) {
		m_owner = owner;
		m_personality = personality;
	}

	public bool Update() {
		OnRiseEqsEvents = null;
		m_personality.RunTests();

		if(OnRiseEqsEvents!=null) {
            Debug.Log("avvenuto");
            OnRiseEqsEvents(this);
			return true;
		}
		else {
			return false;
		}
	}
}
