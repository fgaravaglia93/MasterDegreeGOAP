using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EQS Data/Trait")]
public class Trait : ScriptableObject
{
	public new string name;

	public List<Test> EQSTests = new List<Test>();

	public void RunTestsForTrait(Personality personality, TraitData traitData) {
		for(int i = 0; i < EQSTests.Count; i++) {
			traitData.m_outcomeResults[i] = EQSTests[i].RunTest(personality, traitData, i);
		}
	}
}
