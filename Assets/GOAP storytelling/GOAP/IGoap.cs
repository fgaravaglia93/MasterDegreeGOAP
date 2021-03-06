using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGoap
{
	HashSet<KeyValuePair<string, bool>> getWorldState();

	Goal CreateGoalStates();
	GoalStack GetAllGoals();

	void PlanFound(HashSet<KeyValuePair<string, bool>> goal, Queue<GoapAction> actions);

	void PlanFailed(HashSet<KeyValuePair<string, bool>> failedGoal);

	void PlanAborted(GoapAction aborter);

	void ActionsFinished();

	bool MoveAgent(GoapAction nextAction);
}
