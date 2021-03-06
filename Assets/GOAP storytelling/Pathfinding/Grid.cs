﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Grid : MonoBehaviour {

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public int obstacleProximityPenalty = 5;
    public TerrainType[] walkableRegions;

    public bool displayGridGizmos;
    public int blurPenalty = 3;

    private Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
    private LayerMask walkableMask;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;
    private Node[,] grid;
    private Vector3 worldBottomLeft;

    private int penaltyMin = int.MaxValue;
    private int penaltyMax = int.MinValue;

    public int MaxSize {
        get {
            return gridSizeX * gridSizeY;
        }
    }

	void Awake () {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
     
        foreach(TerrainType region in walkableRegions) {
            walkableMask.value |= region.terrainMask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }
        StartCoroutine(CreateGrid());

    }

    IEnumerator CreateGrid() {
        grid = new Node[gridSizeX, gridSizeY];
        worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        while (true) {
            for (int x = 0; x < gridSizeX; x++) {
                for (int y = 0; y < gridSizeY; y++) {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius * 0.5f, unwalkableMask));

                    int movementPenalty = 0;

                    //Ray ray = new Ray(worldPoint + Vector3.forward * -5, Vector3.forward);
                    //RaycastHit hit;
                    Collider2D ray = Physics2D.OverlapPoint(worldPoint, walkableMask);

                    //if (Physics.Raycast(ray, out hit, 10, walkableMask))
                    //    walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    if (ray != null)
                        walkableRegionsDictionary.TryGetValue(ray.gameObject.layer, out movementPenalty);
                    if (!walkable)
                        movementPenalty += obstacleProximityPenalty;

                    grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                }
            }
            BlurPenaltyMap(blurPenalty);
            yield return new WaitForSeconds(0.3f);
        }
    }

    public List<Node> getNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();

        for(int x=-1; x<=1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                //check world boundry and check impossible diagonals before adding the neighbour
                if (checkX >= 0 && checkY < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    if(grid[checkX,node.gridY].walkable || grid[node.gridX,checkY].walkable)
                        neighbours.Add(grid[checkX, checkY]);
            }
        }
        return neighbours;
    }

    public Node getNodeFromWorldPoint(Vector3 worldPosition) {

        float percentX = (worldPosition.x - transform.position.x) / gridWorldSize.x + 0.5f - (nodeRadius / gridWorldSize.x);
        float percentY = (worldPosition.y - transform.position.y) / gridWorldSize.y + 0.5f - (nodeRadius / gridWorldSize.y);

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY) * percentY);
        return grid[x, y];
    }

    void BlurPenaltyMap(int blurSize) {
        int kernelSize = blurSize * 2 + 1;
        int kernelExtends = (kernelSize - 1) / 2;

        int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
        int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

        for (int y = 0; y < gridSizeY; y++) {
            for(int x=-kernelExtends;x<=kernelExtends;x++) {
                int sampleX = Mathf.Clamp(x, 0, kernelExtends);
                penaltiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
            }
            for(int x=1;x<gridSizeX;x++) {
                int removeIndex = Mathf.Clamp(x - kernelExtends - 1, 0, gridSizeX);
                int addIndex = Mathf.Clamp(x + kernelExtends - 1, 0, gridSizeX-1);

                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
            }
        }
        for (int x = 0; x < gridSizeX; x++) {
            for (int y = -kernelExtends; y <= kernelExtends; y++) {
                int sampleY = Mathf.Clamp(y, 0, kernelExtends);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }

            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            grid[x, 0].movementPenalty = blurredPenalty;

            for (int y = 1; y < gridSizeY; y++) {
                int removeIndex = Mathf.Clamp(y - kernelExtends - 1, 0, gridSizeY);
                int addIndex = Mathf.Clamp(y + kernelExtends - 1, 0, gridSizeY - 1);

                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y-1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                grid[x, y].movementPenalty = blurredPenalty;

                if (blurredPenalty > penaltyMax)
                    penaltyMax = blurredPenalty;
                if (blurredPenalty < penaltyMin)
                    penaltyMin = blurredPenalty;
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y,1f));

        if(grid!=null && displayGridGizmos) {
            foreach(Node n in grid) {
                Gizmos.color = Color.Lerp(Color.white, Color.black,Mathf.InverseLerp(penaltyMin,penaltyMax,n.movementPenalty));
                Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-0.1f));
            }
        }
    }

    [System.Serializable]
    public class TerrainType {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}