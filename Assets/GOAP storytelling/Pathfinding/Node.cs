﻿using UnityEngine;

public class NodeGraphPathFind : IHeapItem<NodeGraphPathFind> {

    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int movementPenalty;

    //distance from Strat
    public int gCost;
    //distance from End
    public int hCost;

    public NodeGraphPathFind parent;

    int heapIndex;

    public int fCost {
        get { return gCost + hCost;}
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public NodeGraphPathFind(bool walkable, Vector3 worldPosition, int gridX, int gridY,int movementPenalty) {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
        this.movementPenalty = movementPenalty;
    }

    public int CompareTo(NodeGraphPathFind nodeToCompare) {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare==0)
            compare = hCost.CompareTo(nodeToCompare.hCost);
        return -compare;
    }
}
