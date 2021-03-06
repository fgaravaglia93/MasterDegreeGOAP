using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAngle : Test
{
	public Transform target;

	public TestAngle() {
		maxValue = 180f;
		resultType = TestResultType.Numerical;
	}

	public override bool RunTest(Personality personality, TraitData traitData, int currentTest) {

		//target = traitData.m_testsRuntimeVariables[currentTest].m_angleTarget != null ? traitData.m_testsRuntimeVariables[currentTest].m_angleTarget : target;
		target = traitData.m_testsRuntimeVariables[currentTest].m_angleTarget;

		if(isActive && target != null) {
			float distance = Vector3.Distance(target.position, personality.m_agent.transform.position);

			if(distance <= traitData.radius) {
				Vector3 a = (target.position - personality.m_agent.transform.position).normalized;
				Vector3 b = personality.m_agent.transform.forward;

				//left hand rule, for the right hand rule change Vector3.Cross(b,a)->Vector3.Cross(a,b)
				float angle = Mathf.Rad2Deg * Mathf.Atan2(Vector3.Dot(Vector3.Cross(b, a), Vector3.up), Vector3.Dot(a, b));

				traitData.m_testResults[currentTest] = angle;
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
