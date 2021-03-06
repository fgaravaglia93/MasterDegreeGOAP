using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate float HeuristicFunction(TileNode from, TileNode to);

public class AStarSolverGrid
{


    public static List<TileNode> Solve(TileNode[,] grid, int gridSizeX, int gridSizeY, TileNode start, TileNode goal, HeuristicFunction heuristic)
    {



       

        List<TileNode> openSet = new List<TileNode>();
        List<TileNode> closedSet = new List<TileNode>();
        List<TileNode> path = new List<TileNode>();
        int desc = 100;
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            TileNode currentNode = openSet[0];
            //calculate min fnode
            for (int i = 1; i < openSet.Count; i++)
            {

                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == goal)
            {
                //RetracePath
                while (currentNode != start)
                {
                    desc--;
                    currentNode.description = ""+desc+"";
                    path.Add(currentNode);
                    currentNode = currentNode.parent;
                }

                path.Reverse();
                
                return path;
                //return path.ToArray();
            }


            //in adj are included node close on the diagonal movement
            List<TileNode> adjList = GetNeighbours(currentNode, grid, gridSizeX, gridSizeY);
            foreach (TileNode adj in adjList)
            {
                if (!adj.walkable || closedSet.Contains(adj))
                {
                    continue;
                }
                float costToAdj = currentNode.gCost + heuristic(currentNode, adj); //heuristic?

                if (costToAdj < adj.gCost || !openSet.Contains(adj))
                {
                    adj.gCost = costToAdj;
                    adj.hCost = heuristic(adj, goal);
                    adj.parent = currentNode;

                    if (!openSet.Contains(adj))
                        openSet.Add(adj);
                }
            }
        }

        //return path.ToArray();
        
        return path;


    }

    private static List<TileNode> GetNeighbours(TileNode node, TileNode[,] grid, int gridSizeX, int gridSizeY)
    {
        List<TileNode> neighbours = new List<TileNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //second condition from d8 to d4
                if ((x == 0 && y == 0) || (Mathf.Abs(x) == Mathf.Abs(y)))
                    continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                //manage nodes on the edge of the grid
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }

        }
        return neighbours;
    }
}













