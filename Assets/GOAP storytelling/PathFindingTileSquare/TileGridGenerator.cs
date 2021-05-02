using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGridGenerator : MonoBehaviour {

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    TileNode[,] grid;

    private int gridSizeX, gridSizeY;
    private float nodeDiameter;
    private List<TileNode> gizmospath;
    [HideInInspector]
    public TileNode check;
    public Tilemap tilemapUnwalkable;

    // Use this for initialization
    void Start () {
        nodeDiameter = nodeRadius * 2;
        //number of nodes for rows and columns
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public TileNode[,] CreateGrid()
    {
        grid = new TileNode[gridSizeX, gridSizeY];
        //this is based the game object containing this script is in the center of the map
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) +
                                     Vector2.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapBox(worldPoint, new Vector2(nodeRadius,nodeRadius), 0 ,unwalkableMask));

                //Debug.Log(tilemapUnwalkable.GetTile(new Vector3Int((int)worldPoint.x, (int)worldPoint.y, 0)));
                if (tilemapUnwalkable.GetTile(new Vector3Int((int)worldPoint.x, (int)worldPoint.y-1, 0)) != null)
                //if (tilemapUnwalkable.GetTile(new Vector3Int(x, y , 0)) != null)
                    walkable = false;


                grid[x, y] = new TileNode(walkable, worldPoint, x, y);
            }
        }
        return grid;
    }
    public void SetGizmosPath(List<TileNode> path)
    {
        if (path != null)
        {
            int i = 0;
            gizmospath = path;
            foreach(TileNode n in gizmospath)
            {
                i ++;
            }
        }
        else
            Debug.Log("path non trovato");
    }


    public TileNode NodeFromWorldPoint(Vector2 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        
        return grid[x, y];
    }


    public int GetSizeX()
    {
        return gridSizeX;
    }

    public int GetSizeY()
    {
        return gridSizeY;
    }

    public TileNode[,] GetGrid()
    {
        return grid;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null)
        {
            foreach (TileNode n in grid)
            {

                if (gizmospath != null)
                    if (gizmospath.Contains(n))
                    {
                       
                        //Gizmos.color = Color.blue;
                        Gizmos.color = new Color(0, 0, 1, 0.5f);
                        Gizmos.DrawCube(n.worldPosition, Vector2.one * (nodeDiameter - .1f));
                    }
            }
            foreach (TileNode n in grid)
            {

                if (!n.walkable)
                {
                    //Gizmos.color = Color.red;
                    Gizmos.color = new Color(1, 0, 0, 0.5f);
                    Gizmos.DrawCube(n.worldPosition, Vector2.one * (nodeDiameter - .1f));
                }

                if(check != null)
                    if((n.gridX == check.gridX) && (n.gridY == check.gridY))
                    {
                        Gizmos.color = new Color(0, 1, 0, 0.5f);
                        Gizmos.DrawCube(n.worldPosition, Vector2.one * (nodeDiameter - .1f));
                    }
            }     
                

        }
        
    }
}
