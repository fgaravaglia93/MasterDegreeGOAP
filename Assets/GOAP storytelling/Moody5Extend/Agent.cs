using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Agent : MonoBehaviour
{
	private FSM m_fsm;
	private FSM.FSMState m_idleState;
	private FSM.FSMState m_moveToState;
	private FSM.FSMState m_performActionState;

    [HideInInspector]
    public Goal m_initialGoal;
    [SerializeField]
	public GoalStack m_goalStack;

    [HideInInspector]
    public Queue<GoapAction> m_currentActions;

	public HashSet<GoapAction> m_availableActions;


	private IGoap m_dataProvider;
	private GoapPlanner m_planner;

	public EQSAgent m_eqsAgent;

	public Personality m_personality;

	[HideInInspector]
	public List<Trait> m_traits;
    public TraitData m_eqsEventOccurred;
    [HideInInspector]
    public Queue<GoapAction> plan;

    [HideInInspector]
    public string planListText;
    [HideInInspector]
    public string actionText;
    [HideInInspector]
    public string goalText;
    [HideInInspector]
    public Color goalColor;
    [HideInInspector]
    public Color actionColor;
    

    bool waiting;

    void Awake() {

		m_fsm = new FSM();
		m_availableActions = new HashSet<GoapAction>();
		m_currentActions = new Queue<GoapAction>();
		m_planner = new GoapPlanner();
		m_dataProvider = GetComponent<IGoap>();
		m_initialGoal = m_dataProvider.CreateGoalStates();
		m_goalStack = new GoalStack(m_initialGoal);
        m_goalStack = m_dataProvider.GetAllGoals();

		LoadActions();
		m_idleState = IdleState;
		m_performActionState = PerformActionState;
		m_moveToState = MoveToState;
		m_fsm.pushState(m_idleState);

		m_personality.Init(this);
		RecalculateTraitList();
		m_eqsAgent = new EQSAgent(this, m_personality);
        
	}

    public virtual void Update() {

        
        //m_eqsEventOccurred = m_eqsAgent.Update();
        //m_fsm.Update(gameObject);
        //Debug.Log("goal cost " + m_goalStack.Peek().maxCostForGoal);
        if(m_goalStack.Peek() != null)
        {
            goalText = "Goal:" + m_goalStack.Peek().m_nameGoal;
            goalColor = new Color(1, 1, 1);
            DisplayManager.instance.ShowOnConsoleGoal(goalText);
            m_eqsEventOccurred = m_eqsAgent.Update();
            m_fsm.Update(gameObject);
            if (m_currentActions.Count <= 0)
            {
                m_goalStack.Remove(m_goalStack.Peek());
                goalColor = new Color(0, 1, 0);
                //Goal reached --> Set NPC mood to Joy
                DisplayManager.instance.ChangeMood(gameObject, MoodType.Joy, 10f);
                DisplayManager.instance.ShowOnConsoleGoal(goalText, goalColor);
            }
        }  
	}

	public void AddAction(GoapAction action) {
		m_availableActions.Add(action);
	}

	public GoapAction GetAction(Type actionType) {
		foreach(GoapAction action in m_availableActions) {
			if(action.GetType().Equals(actionType))
				return action;
		}
		return null;
	}

	public void RemoveAction(GoapAction action) {
		Destroy(action);
		m_availableActions.Remove(action);
	}


	public void RecalculateTraitList() {
		m_traits = new List<Trait>();
		//m_goalStack = new GoalStack(m_initialGoal);

		foreach(TraitData traitData in m_personality.m_traitDatas) {
            if(traitData.trait != null)
            {
                traitData.name = traitData.trait.name;
                m_traits.Add(traitData.trait);
            }
			
			/*Type type = Type.GetType(traitData.m_influenceType.goalType);
			if(type != null) {
				traitData.m_influenceType.goal = (Goal)Activator.CreateInstance(type);
			}*/
		}
	}

	public void RecalculateTestsRuntimeVariables() {
		foreach(TraitData traitData in m_personality.m_traitDatas) {
            if(traitData.trait != null)
			    traitData.Init(m_eqsAgent);
			/*Type type = Type.GetType(traitData.m_influenceType.goalType);
			if(type != null) {
				traitData.m_influenceType.goal = (Goal)Activator.CreateInstance(type);
			}*/
		}
	}

	private bool HasActionPlan() {
		return m_currentActions.Count > 0;
	}

	public virtual void IdleState(FSM fsm, GameObject agent) {

        HashSet<KeyValuePair<string, bool>> worldState = m_dataProvider.getWorldState();
		Goal goal = m_goalStack.Peek();
		plan = m_planner.Plan(agent, m_availableActions, worldState, goal.GoalStates);
        Moody5Action acti = (Moody5Action)plan.Peek();
        if (plan != null) {
            Debug.Log("<color=blue>Found Plan:</color>" + PrettyPrint(goal.GoalStates));
            m_currentActions = plan;
			m_dataProvider.PlanFound(goal.GoalStates, plan);

			//Move To _performActionState
			fsm.popState();
			fsm.pushState(m_performActionState);
		}
		else {
			Debug.Log("<color=orange>Failed Plan:</color>" + PrettyPrint(goal.GoalStates));
			m_dataProvider.PlanFailed(goal.GoalStates);
			//Stay In _idleState
		}
	}
	//make it to switch goal when all action for one goal are completed?
	public virtual void PerformActionState(FSM fsm, GameObject agent) {

        if(!waiting)
        {
            if (m_eqsEventOccurred != null)
            {
                Debug.Log("<color=yellow>EQS Event Occurred: Recaculate Plan</color>");
                fsm.popState();
                fsm.pushState(m_idleState);
                return;
            }
            if (!HasActionPlan())
            {
                Debug.Log("<color=red>Done actions</color>");
                fsm.popState();
                fsm.pushState(m_idleState);
                m_dataProvider.ActionsFinished();
                return;
            }

            Moody5Action action = (Moody5Action)m_currentActions.Peek();
            actionText = action.console;
            actionColor = new Color(1, 1, 1);
            DisplayManager.instance.ShowOnConsoleAction(actionText);
            if (action.IsDone())
            {
                if (action.CalculateSuccess())
                {
                    m_currentActions.Dequeue();
                    actionColor = new Color(0, 1, 0);
                    actionText = "Done";
                    DisplayManager.instance.ShowOnConsoleAction(actionText, actionColor);
                }
                else
                {
                    actionText = "Action failed, repeat";
                    DisplayManager.instance.ShowOnConsoleAction(actionText, new Color(255, 0, 0));
                    //StartCoroutine("WaitFor");
                    //waiting = true;
                    action.OnReset();
                }

            }

            if (HasActionPlan())
            {

                action = (Moody5Action)m_currentActions.Peek();
                bool inRange = action.RequiresInRange() ? action.InRange : true;
                if (inRange)
                {
                    bool success = action.Perform(agent);

                    if (!success)
                    {
                        fsm.popState();
                        fsm.pushState(m_idleState);
                        m_dataProvider.PlanAborted(action);
                    }
                }
                else
                {
                    fsm.pushState(m_moveToState);
                }
            }
            else
            {
                fsm.popState();
                fsm.pushState(m_idleState);
                m_dataProvider.ActionsFinished();
            }
        }
       
	}

	public virtual void MoveToState(FSM fsm, GameObject agent) {

        if(m_eqsEventOccurred!=null) {
            GetComponent<MoveToNextAction>().followPath = false;
            GetComponent<HogwartsStudent>().pathCalculated = false;
            Debug.Log("<color=yellow>EQS Event Occurred: Recaculate Plan</color>");
            var traitText = GetComponent<MoodController>().textTrait;
            var traitColor = GetComponent<MoodController>().colorTrait;
            traitText = m_eqsEventOccurred.name;
            traitColor = new Color(0, 1, 0);
            DisplayManager.instance.ShowOnConsoleTraitAdHoc(traitText, traitColor);
            GetComponent<MoodController>().textTrait = "Trait ad hoc";
            GetComponent<MoodController>().colorTrait = new Color(1, 1, 1);
            
            //10f to secure mood activation
            //Debug.Log(m_eqsEventOccurred.changeToMood);
            DisplayManager.instance.ChangeMood(gameObject, DisplayManager.instance.moodDict[m_eqsEventOccurred.changeToMood], 10f);
            fsm.popState(); //move
			fsm.popState(); //perform
			fsm.pushState(m_idleState);
			return;
		}

		GoapAction action = m_currentActions.Peek();
        //Debug.Log("Action: " + action.nameAction);
        if (action.RequiresInRange() && action.target == null) {

			Debug.Log("<color=red>Fatal error:</color> Action requires a target but has none. Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()");
            
            fsm.popState(); //move
			fsm.popState(); //perform
			fsm.pushState(m_idleState);
			return;
		}

		if(m_dataProvider.MoveAgent(action)) {
			fsm.popState();
            Debug.Log("Move to state");
		}
        
	}

	private void FindDataProvider() {
		foreach(Component component in GetComponents(typeof(Component))) {
			if(typeof(IGoap).IsAssignableFrom(component.GetType())) {
				m_dataProvider = (IGoap)component;
				return;
			}
		}
	}

	private void LoadActions() {
		GoapAction[] actions = GetComponents<GoapAction>();
		foreach(GoapAction action in actions) {
			m_availableActions.Add(action);
		}
		Debug.Log("Found actions: " + PrettyPrint(actions));
	}

    IEnumerator CollectAnimation(float t) {
        GetComponentInChildren<Animator>().SetFloat("speed", 0f);
        yield return new WaitForSeconds(t);
        GetComponentInChildren<Animator>().SetFloat("speed", 1f);
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

    IEnumerable WaitFor()
    {
        yield return new WaitForSeconds(1f);
        waiting = false;
    }

  


#if UNITY_EDITOR

    private void OnDrawGizmos() {

		if(m_personality!=null && m_personality.m_traitDatas != null) {
			foreach(TraitData traitData in m_personality.m_traitDatas) {
					Gizmos.color = Color.HSVToRGB((traitData.radius * 0.15f), 1.0f, 1.0f);
					Gizmos.DrawWireSphere(transform.position, traitData.radius);
					//UnityEditor.Handles.Label(transform.position, traitData.trait.name);
			}
		}
	}

#endif
}
