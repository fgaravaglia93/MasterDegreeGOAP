using System.Collections.Generic;
using UnityEngine;

public class FSM
{
	private Stack<FSMState> statesStack = new Stack<FSMState>();

	public delegate void FSMState(FSM fsm, GameObject gameObject);

	public void Update(GameObject gameObject) {
		if(statesStack.Peek() != null)
			statesStack.Peek().Invoke(this, gameObject);
	}

	public void pushState(FSMState state) {
		statesStack.Push(state);
	}

	public void popState() {
		statesStack.Pop();
	}
}
