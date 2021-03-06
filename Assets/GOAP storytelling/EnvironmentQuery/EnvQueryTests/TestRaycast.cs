using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRaycast : Test
{
	public Transform target;

	public TestRaycast() {
		resultType = TestResultType.Boolean;
	}

	public override bool RunTest(Personality personality, TraitData traitData, int currentTest) {

		target = traitData.m_testsRuntimeVariables[currentTest].m_raycastTarget;

		if(isActive && target!=null) {
			Vector3 startPosition = personality.m_agent.transform.position;

			Physics.Raycast(startPosition, target.position - startPosition, out RaycastHit hit, traitData.radius);

			if(hit.transform == target) {
				traitData.m_testResults[currentTest] = 1f;
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
