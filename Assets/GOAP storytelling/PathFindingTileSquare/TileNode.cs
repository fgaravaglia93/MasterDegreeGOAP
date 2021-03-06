
using UnityEngine;

public class TileNode
{

    public bool walkable;
    public Vector2 worldPosition;

    public int gridX;
    public int gridY;

    public TileNode parent;
    public float gCost;
    public float hCost;

   // public GameObject sceneObject;
    public string description;

    public TileNode(bool walkable, Vector2 worldPosition, int gridX, int gridY, GameObject o = null)
    {

      
        this.worldPosition = worldPosition;
        this.walkable = walkable;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public float fCost
    {
        get {
            return gCost + hCost;
        }
    }

   

    
}