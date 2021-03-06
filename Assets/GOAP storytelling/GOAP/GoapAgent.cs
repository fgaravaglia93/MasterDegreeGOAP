using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class GoapAgent : MonoBehaviour
{
	private FSM _fsm;

	private FSM.FSMState _idleState;
	private FSM.FSMState _performActionState;
	private FSM.FSMState _moveToState;

	private HashSet<GoapAction> _availableActions;
	private Queue<GoapAction> _currentActions;

	private IGoap _dataProvider;

	private GoapPlanner _planner;

    void Start()
    {
		_fsm = new FSM();
		_availableActions = new HashSet<GoapAction>();
		_currentActions = new Queue<GoapAction>();
		_planner = new GoapPlanner();
		_dataProvider = GetComponent<IGoap>();

		LoadActions();

		_idleState = IdleState;
		_performActionState = PerformActionState;
		_moveToState = MoveToState;
		_fsm.pushState(_idleState);
	}

    void Update()
    {
		_fsm.Update(gameObject);
    }

	public void AddAction(GoapAction action) {
		_availableActions.Add(action);
	}

	public GoapAction getAction(Type actionType) {
		foreach(GoapAction action  in _availableActions) {
			if(action.GetType().Equals(actionType))
				return action;
		}
		return null;
	}

	public void RemoveAction(GoapAction action) {
		_availableActions.Remove(action);
	}

	bool HasActionPlan() {
		return _currentActions.Count > 0;
	}

	void IdleState(FSM fsm, GameObject agent) {

		HashSet<KeyValuePair<string, bool>> worldState = _dataProvider.getWorldState();

		//temp
		HashSet<KeyValuePair<string, bool>> goal = _dataProvider.CreateGoalStates().GoalStates;

		Queue<GoapAction> plan = _planner.Plan(agent, _availableActions, worldState, goal);

		if(plan != null) {
			_currentActions = plan;
			_dataProvider.PlanFound(goal, plan);

			//Move To _performActionState
			fsm.popState();
			fsm.pushState(_performActionState);
		}
		else {
			Debug.Log("<color=orange>Failed Plan:</color>" + PrettyPrint(goal));
			_dataProvider.PlanFailed(goal);
			//Stay In _idleState
		}
	}

	void PerformActionState(FSM fsm, GameObject agent) {

		if(!HasActionPlan()) {
			Debug.Log("<color=red>Done actions</color>");
			fsm.popState();
			fsm.pushState(_idleState);
			_dataProvider.ActionsFinished();
			return;
		}

		GoapAction action = _currentActions.Peek();

		if(action.IsDone()) {
			_currentActions.Dequeue();
		}

		if(HasActionPlan()) {

			action = _currentActions.Peek();
			bool inRange = action.RequiresInRange() ? action.InRange : true;

			if(inRange) {
				bool success = action.Perform(agent);

				if(!success) {
					fsm.popState();
					fsm.pushState(_idleState);
					_dataProvider.PlanAborted(action);
				}
			}
			else {
				fsm.pushState(_moveToState);
			}
		}
		else {
			fsm.popState();
			fsm.pushState(_idleState);
			_dataProvider.ActionsFinished();
		}
	}

	void MoveToState(FSM fsm, GameObject agent) {

		GoapAction action = _currentActions.Peek();

		if(action.RequiresInRange() && action.target == null) {

			Debug.Log("<color=red>Fatal error:</color> Action requires a target but has none. Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()");
			fsm.popState(); //move
			fsm.popState(); //perform
			fsm.pushState(_idleState);
			return;
		}

		if(_dataProvider.MoveAgent(action)) {
			fsm.popState();
		}

	}

	void LoadActions() {
		GoapAction[] actions = GetComponents<GoapAction>();
		foreach(GoapAction action in actions) {
			_availableActions.Add(action);
		}
		Debug.Log("Found actions: " + PrettyPrint(actions));
	}

	public static string PrettyPrint(HashSet<KeyValuePair<string, bool>> state) {
		string s = "";
		foreach(KeyValuePair<string, bool> kvp in state) {
			s += kvp.Key + ":" + kvp.Value.ToString();
			s += ", ";
		}
		return s;
	}

	public static string PrettyPrint(Queue<GoapAction> actions) {
		string s = "";
		foreach(GoapAction action in actions) {
			s += action.GetType().Name;
			s += "-> ";
		}
		s += "GOAL";
		return s;
	}

	public static string PrettyPrint(GoapAction[] actions) {
		string s = "";
		foreach(GoapAction action in actions) {
			s += action.GetType().Name;
			s += ", ";
		}
		return s;
	}

	public static string PrettyPrint(GoapAction action) {
		return action.GetType().Name;
	}
}
