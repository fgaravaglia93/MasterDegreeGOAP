using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Response { CostManipulation, PathfindingManipulation, ActionManipulation, GoalManipulation };
public enum ResponseType {Add, Remove};

[System.Serializable]
public struct InfluenceType
{
	public Response response;
	public ResponseType responseType;

	public float costIncrement;
	[HideInInspector]
	public bool costModified;

    //public Labourer labourer;

	public string actionType;

	public string goal;
	//public Goal goal;
}
