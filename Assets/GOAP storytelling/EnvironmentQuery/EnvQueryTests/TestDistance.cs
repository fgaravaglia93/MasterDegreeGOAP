using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDistance : Test
{
	public Transform target;

	public TestDistance() {
		resultType = TestResultType.Numerical;
	}

	public override bool RunTest(Personality personality, TraitData traitData, int currentTest) {

		target = traitData.m_testsRuntimeVariables[currentTest].m_distanceTarget;

		if(isActive && target!=null ) {
			float distance = Vector3.Distance(target.position, personality.m_agent.transform.position);
			if(distance <= traitData.radius) {
				traitData.m_testResults[currentTest] = Vector3.Distance(target.position, personality.m_agent.transform.position);
				return true;
			}
			else {
				traitData.m_testResults[currentTest] = 0f;
				return false;
			}
		}
		else {
			traitData.m_testResults[currentTest] = 0f;
			if(!isActive)
				return true;
			return false;
		}
	}
}
