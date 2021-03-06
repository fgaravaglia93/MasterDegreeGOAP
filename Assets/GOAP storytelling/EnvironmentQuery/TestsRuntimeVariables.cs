using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TestsRuntimeVariables
{
	public string name;
	//public string m_testName;
	public int m_numberOfDynamicVariables;

	public bool m_testAngle;
	public bool m_testDistance;
	public bool m_testRaycast;
	public bool m_testTag;

	//Test Angle
	[Tooltip("The Transform Of The Game Objects To Calculate The Angle With")]
	public Transform m_angleTarget;

	//Test Distance
	[Tooltip("The Transform Of The Game Objects To Calculate The Distance With")]
	public Transform m_distanceTarget;

	//Test Raycast
	[Tooltip("The Transform Of The Game Objects To Cast The Raycast To")]
	public Transform m_raycastTarget;

	//Test Tag
	[Tooltip("The Tag Of The Game Objects To Check")]
	public string m_tagToCheck;

	public TestsRuntimeVariables(string testName, Type testType, List<object> values) {

		if(values != default(List<object>))
			SetOldValues(values);

		name = testName;
		//m_testName = testName;

		if(testType.Equals(typeof(TestAngle))) {
			m_testAngle = true;
			m_testDistance = false;
			m_testRaycast = false;
			m_testTag = false;
			m_numberOfDynamicVariables = 2;
		}
		else if(testType.Equals(typeof(TestDistance))) {
			m_testAngle = false;
			m_testDistance = true;
			m_testRaycast = false;
			m_testTag = false;
			m_numberOfDynamicVariables = 2;
		}
		else if(testType.Equals(typeof(TestRaycast))) {
			m_testAngle = false;
			m_testDistance = false;
			m_testRaycast = true;
			m_testTag = false;
			m_numberOfDynamicVariables = 2;
		}
		else if(testType.Equals(typeof(TestTag))) {
			m_testAngle = false;
			m_testDistance = false;
			m_testRaycast = false;
			m_testTag = true;
			m_numberOfDynamicVariables = 2;
		}
	}

	public void  SetOldValues(List<object> values) {
		System.Reflection.FieldInfo[] fieldInfos = GetType().GetFields();
		for(int i = 0; i < fieldInfos.Length; i++) {
			fieldInfos[i].SetValue(this, values[i]);
		}
	}
}
