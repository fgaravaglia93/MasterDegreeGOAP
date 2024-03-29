﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    Grid grid;
    public GameObject hexDrawer;

    private void Awake() {
        grid = GetComponent<Grid>();
    }

    public void FindPath(PathRequest request, Action<PathResult> callback) {

        //Vector3[] tmp = HexUtilities.GetPathBetweenTwoCubes();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        /*Node startNode = grid.getNodeFromWorldPoint(request.pathStart);
        Node targetNode = grid.getNodeFromWorldPoint(request.pathEnd);
        targetNode.walkable = true;
        //startNode.walkable = true;

            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0) {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode) {
                    pathSuccess = true;
                    targetNode.walkable = false;
                    break;
                }

                foreach (Node neighbour in grid.getNeighbours(currentNode)) {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    int newCostToNeighbour = currentNode.gCost + getDistance(currentNode, neighbour)+neighbour.movementPenalty;
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = getDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }

        if (pathSuccess) {
            waypoints = RetracePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }*/
        //waypoints = tmp;
        pathSuccess = true;
       
        callback(new PathResult(waypoints, pathSuccess,request.callback));
    }

    Vector3[] RetracePath(NodeGraphPathFind startNode,NodeGraphPathFind endNode) {
        List<NodeGraphPathFind> path = new List<NodeGraphPathFind>();
        NodeGraphPathFind currentNode = endNode;

        while(currentNode!= startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(startNode);
        List<Vector3> waypoints = new List<Vector3>();

        for (int i = 0; i < path.Count; i++) {
            waypoints.Add(path[i].worldPosition);
        }
        waypoints.Reverse();
        return waypoints.ToArray();
    }

    Vector3[] SimplifyPath(List<NodeGraphPathFind> path) {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;
        
        bool once = false;
        for (int i=1; i<path.Count;i++) {
            Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
            if (directionNew != directionOld) {
                waypoints.Add(path[i - 1].worldPosition);
                if(!once) {
                    waypoints.Add(path[1].worldPosition);
                    once = true;
                }
            }

            directionOld = directionNew;
        }
        
        return waypoints.ToArray();
    }

    //The Heuristic
    int getDistance (NodeGraphPathFind nodeA, NodeGraphPathFind nodeB) {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }
}
