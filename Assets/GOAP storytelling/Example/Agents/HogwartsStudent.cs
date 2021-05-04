using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * A general labourer class.
 * You should subclass this for specific Labourer classes and implement
 * the createGoalState() method that will populate the goal for the GOAP
 * planner.
 */

public abstract class HogwartsStudent : MonoBehaviour, IGoap
{
    const float minPathUpdateTime = 0.3f;
    const float pathUpdateMoveThreshold = 0.5f;

    public float moveSpeed = 1;
    public bool workstationFree;
    public bool targetReached;


    

    //Emotions and change on actions parameters
    public float durationActionInfluence = 1;
    public float successActionInfluence = 1;

    //Movement on a Tilemap
    public TileNode start;
    public TileNode end;
    public bool pathCalculated;

    /**
	 * Key-Value data that will feed the GOAP actions and system while planning.
	 */
    public HashSet<KeyValuePair<string, bool>> getWorldState()
    {
        HashSet<KeyValuePair<string, bool>> worldData = new HashSet<KeyValuePair<string, bool>>();

   
        // worldData.Add(new KeyValuePair<string, bool>("hasSomething, (condition)));
        worldData.Add(new KeyValuePair<string, bool>("hasIngredient", false));
        worldData.Add(new KeyValuePair<string, bool>("recipeIsKnown", false));
        worldData.Add(new KeyValuePair<string, bool>("workstationIsFree", workstationFree));
        worldData.Add(new KeyValuePair<string, bool>("preparedIngredient", false));
        worldData.Add(new KeyValuePair<string, bool>("potIsFree", true));
        worldData.Add(new KeyValuePair<string, bool>("potionIsReady", false));
        worldData.Add(new KeyValuePair<string, bool>("potionIsDelivered", false));
        return worldData;
    }

    /**
	 * Implement in subclasses
	 */

    public abstract Goal CreateGoalStates();
    public abstract GoalStack GetAllGoals();

    public void PlanFailed(HashSet<KeyValuePair<string, bool>> failedGoal)
    {
        // Not handling this here since we are making sure our goals will always succeed.
        // But normally you want to make sure the world state has changed before running
        // the same goal again, or else it will just fail.
    }

    public void PlanFound(HashSet<KeyValuePair<string, bool>> goal, Queue<GoapAction> actions)
    {
        // Yay we found a plan for our goal
        //Debug.Log ("<color=green>Plan found</color> "+GoapAgent.prettyPrint(actions));
    }

    public void ActionsFinished()
    {
        // Everything is done, we completed our actions for this gool. Hooray!
        Debug.Log ("<color=blue>Actions completed</color>");
    }

    public void PlanAborted(GoapAction aborter)
    {
        // An action bailed out of the plan. State has been reset to plan again.
        // Take note of what happened and make sure if you run the same goal again
        // that it can succeed.
        //Debug.Log ("<color=red>Plan Aborted</color> "+GoapAgent.prettyPrint(aborter));
    }

    
    //qui va spostato lo script del movimento col pathfinding
    public bool MoveAgent(GoapAction nextAction)
    {
        // How the NPC move to another action target
        // move towards the NextAction's target
        float step = moveSpeed * Time.deltaTime;

        //Make a permanent state of the last calculated pathfinding and update it only if destination change
        //this should help with keeping the movement fluid on the same path

        //GetComponent<MoveToNextAction>().PathToNextAction(nextAction.target.transform);
        
        if (nextAction.target == null)
            return false;
        
        if (!pathCalculated)
        {
           
            GetComponent<MoveToNextAction>().PathToNextAction(nextAction.target.transform);
            //Debug.Log("Calculate Path" + nextAction.target.transform.position);
            
            pathCalculated = true;
        }

        //true if a collision between target and person has been detected
        if (targetReached)
        {
            nextAction.InRange = true;
            pathCalculated = false;
            targetReached = false;
            return true;
        }
        else
        {
            return false;
        }

    }

 
    
    
}

