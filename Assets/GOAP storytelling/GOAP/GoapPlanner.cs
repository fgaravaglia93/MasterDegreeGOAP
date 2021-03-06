using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoapPlanner
{
    public Queue<GoapAction> Plan(GameObject agent, HashSet<GoapAction> availableActions, HashSet<KeyValuePair<string,bool>> worldState, HashSet<KeyValuePair<string, bool>> goal) {

		foreach(GoapAction action in availableActions) {
			action.Reset();
		}

		HashSet<GoapAction> usableActions = new HashSet<GoapAction>();

		foreach(GoapAction action in availableActions) {
			if(action.CheckProceduralPrecondition(agent))
				usableActions.Add(action);
		}

		List<Node> leaves = new List<Node>();

		Node start = new Node(null, 0, worldState, null);
		bool success = BuildGraph(start, leaves, usableActions, goal);

		if(!success) {
			Debug.Log("NO PLAN");
			return null;
		}

		Node cheapest = null;
		foreach(Node leaf in leaves) {
			if(cheapest == null)
				cheapest = leaf;
			else {
				if(leaf.runningCost < cheapest.runningCost)
					cheapest = leaf;
			}
		}

		List<GoapAction> result = new List<GoapAction>();
		Node node = cheapest;
		while(node!=null) {
			if(node.action!=null) {
				result.Insert(0, node.action);
			}
			node = node.parent;
		}

		Queue<GoapAction> queue = new Queue<GoapAction>();
		foreach(GoapAction action in result) {
			queue.Enqueue(action);
		}

		return queue;
	}

	bool BuildGraph(Node parent, List<Node> leaves, HashSet<GoapAction> usableActions, HashSet<KeyValuePair<string, bool>> goal) {

		bool foundOne = false;

		foreach(GoapAction action in usableActions) {
			if(InState(action.Preconditions, parent.state)) {
				HashSet<KeyValuePair<string, bool>> currentState = PopulateState(parent.state, action.Effects);

				Node node = new Node(parent, parent.runningCost + action.cost + action.eqsCost, currentState, action);

				if(InState(goal, currentState)) {
					leaves.Add(node);
					foundOne = true;
				}
				else {
					HashSet<GoapAction> subset = ActionSubset(usableActions, action);
					bool found = BuildGraph(node, leaves, subset, goal);
					if(found)
						foundOne = true;
				}
			}
		}

		return foundOne;
	}

	HashSet<GoapAction> ActionSubset(HashSet<GoapAction> actions, GoapAction actionToRemove) {
		HashSet<GoapAction> subset = new HashSet<GoapAction>();
		foreach(GoapAction action in actions) {
			if(!action.Equals(actionToRemove))
				subset.Add(action);
		}
		return subset;
	}

	bool InState(HashSet<KeyValuePair<string,bool>> test, HashSet<KeyValuePair<string, bool>> state) {
		bool allMatch = true;

		/*foreach(KeyValuePair<string,object> t in test) {
			bool match = false;
			foreach(KeyValuePair<string,object> s in state) {
				if(s.Equals(t)) {
					match = true;
					break;
				}
			}
			if(!match)
				allMatch = false;
		}*/

		allMatch = test.IsSubsetOf(state);

		return allMatch;
	}

	HashSet<KeyValuePair<string, bool>> PopulateState(HashSet<KeyValuePair<string, bool>> currentState, HashSet<KeyValuePair<string, bool>> stateChange) {
		HashSet<KeyValuePair<string, bool>> state = new HashSet<KeyValuePair<string, bool>>();

		foreach(KeyValuePair<string, bool> kvp in currentState) {
			state.Add(new KeyValuePair<string, bool>(kvp.Key, kvp.Value));
		}

		foreach(KeyValuePair<string, bool> change in stateChange) {
			/*bool exists = false;

			foreach(KeyValuePair<string, object> s in state) {
				if(s.Equals(change)) {
					exists = true;
					break;
				}
			}

			if(exists) {
				state.RemoveWhere((KeyValuePair<string, object> kvp) => { return (kvp.Key.Equals(change.Key)); });
				KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key, change.Value);
				state.Add(updated);
			}
			else {
				state.Add(new KeyValuePair<string, object>(change.Key, change.Value));
			}*/

			if(state.Contains(change)) {
				state.RemoveWhere((KeyValuePair<string, bool> kvp) => { return (kvp.Key.Equals(change.Key)); });
			}
			state.Add(new KeyValuePair<string, bool>(change.Key, change.Value));

		}

		return state;
	}

	private class Node
	{
		public Node parent;
		public float runningCost;
		public HashSet<KeyValuePair<string, bool>> state;
		public GoapAction action;

		public Node(Node parent, float runningCost, HashSet<KeyValuePair<string, bool>> state, GoapAction action) {
			this.parent = parent;
			this.runningCost = runningCost;
			this.state = state;
			this.action = action;
		}
	}
}
