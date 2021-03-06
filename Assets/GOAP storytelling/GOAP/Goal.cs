using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Goal
{
    [SerializeField]
    public string m_nameGoal;
	private HashSet<KeyValuePair<string, bool>> _goal = new HashSet<KeyValuePair<string, bool>>();
	public float maxCostForGoal=100f;

	public HashSet<KeyValuePair<string, bool>> GoalStates { get => _goal; set => _goal = value; }

	public Goal(string name) {
        m_nameGoal = name;
        GoalStates = new HashSet<KeyValuePair<string, bool>>();
    }

	public Goal(string name, HashSet<KeyValuePair<string, bool>> states) {
        m_nameGoal = name;
        GoalStates = states;
	}

	public Goal GetGoal() {
		return this;
	}
	public void AddState(KeyValuePair<string,bool> kvp) {
		GoalStates.Add(kvp);
	}
	public void RemoveState(KeyValuePair<string, bool> kvp) {
		GoalStates.Remove(kvp);
	}

    public override bool Equals(object obj) {
        if((obj == null) || !GetType().Equals(obj.GetType())) {
            return false;
        }
        else {
            Goal g = (Goal)obj;
            return GoalStates.IsSubsetOf(g.GoalStates) && !GoalStates.IsProperSubsetOf(g.GoalStates);
        }
    }
    public override int GetHashCode() {
        return GoalStates.GetHashCode();
    }

    /// <summary>
    /// Use this to create the Goal for the agent
    /// Add each Key-Value using goal.Add(new KeyValuePair<string, object>("key", value));
    /// </summary>
    //public virtual Goal CreateGoal() { }

}
