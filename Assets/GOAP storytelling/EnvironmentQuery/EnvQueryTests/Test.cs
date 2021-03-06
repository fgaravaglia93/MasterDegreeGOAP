using System;
using System.Collections.Generic;
using UnityEngine;

public enum EQSTestScoringEquation { Linear, InverseLinear, Square, HalfSine, InverseHalfSine, HalfSineSquared, InverseHalfSineSquared, Sigmoid, InverseSigmoid }
public enum TestResultType { Boolean, Numerical }

public class Test : ScriptableObject
{
	public bool isActive;
	public float weight  = 1f;
	[HideInInspector]
	public float maxValue;

	public EQSTestScoringEquation scoringEquation;
	public TestResultType resultType;

	public Test() {
		isActive = true;
		weight = 1.0f;
		scoringEquation = EQSTestScoringEquation.Linear;
	}

	public virtual bool RunTest(Personality personality, TraitData traitData, int currentTest) { return false; }
}
