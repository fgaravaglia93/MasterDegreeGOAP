using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTag : Test
{
	public string tagToCheck;

	public TestTag() {
		maxValue = 1f;
		resultType = TestResultType.Boolean;
	}

	public override bool RunTest(Personality personality, TraitData traitData, int currentTest) {

		tagToCheck = traitData.m_testsRuntimeVariables[currentTest].m_tagToCheck != null ? traitData.m_testsRuntimeVariables[currentTest].m_tagToCheck : "";
        if (isActive && tagToCheck != "") {
            Collider2D[] colliders = Physics2D.OverlapCircleAll((new Vector2 (personality.m_agent.transform.position.x, personality.m_agent.transform.position.y)), traitData.radius, LayerMask.GetMask("Tests") + LayerMask.GetMask("WalkableLayer") + LayerMask.GetMask("BlockingLayer"));
            tagToCheck = tagToCheck.TrimStart();
            tagToCheck = tagToCheck.TrimEnd();
            for (int i = 0; i < colliders.Length; i++) {

                if (colliders[i].CompareTag(tagToCheck)) {
					traitData.m_testResults[currentTest] = 1f;
                    return true;
				}
			}
			traitData.m_testResults[currentTest] = 0f;
            //traitData.m_occurred = false;
			return false;
		}
		else {
			traitData.m_testResults[currentTest] = 0f;
			if(!isActive)
				return true;
			return false;
		}
	}
}
