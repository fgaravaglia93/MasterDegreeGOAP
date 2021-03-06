using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GoalStack
{
    [SerializeField]
	public List<Goal> m_goals;
	private Goal m_mainGoal;

	public GoalStack() {
		m_goals = new List<Goal>();
	}

	public GoalStack(Goal goal) {
		m_goals = new List<Goal>();
		if(goal != null) {
			m_mainGoal = goal;
			Push(m_mainGoal);
		}
	}

	public void Push(Goal goal) {
		if(m_goals.Contains(goal))
			m_goals.Remove(goal);

		m_goals.Insert(0, goal);
	}

	public Goal Pop () {
		if(m_goals.Count > 0) {
			Goal goal = m_goals[0];
			m_goals.RemoveAt(0);
			return goal;
		}
		else
			return null;

	}
	public Goal Peek() {
		if(m_goals.Count > 0) {
			return m_goals[0];
		}
		else
			return null;
	}

	public void Remove(Goal goal) {
		m_goals.Remove(goal);
	}
	public List<Goal> GetGoals() {
		return m_goals;
	}

	public override string ToString() {
		string s = "";
		foreach(Goal goal in m_goals) {
			s += goal.GetType() + ", ";
		}
		return s;
	}
}
