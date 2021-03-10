using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToNextAction : MonoBehaviour
{

    public GameObject tileGrid;


    public bool stopAtFirstHit = false;

    
    private Vector2 gridWorldSize;
    private float nodeRadius;
    public bool followPath = false;
    private TileNode[,] grid;
    private List<TileNode> path, reversePath;
    
    private int gridSizeX, gridSizeY;

  
    private Vector3 currentPositionHolder;
    public float speed;
    private int currentNode;

    //new for GOAP thesis
    public Transform target;

    // Use this for initialization
    void Start()
    {
        grid = transform.parent.GetComponent<TileGridGenerator>().GetGrid();
    }

    private void Update()
    {

        if (followPath)
        {

            if ((transform.position - currentPositionHolder).magnitude>0.5f)
            {
                transform.position = Vector3.Lerp(transform.position, currentPositionHolder, Time.deltaTime * speed);
            }
            else
            {
              if (currentNode < path.Count - 1)
                {
                    CheckNode();
                    currentNode++;
                }
            }
        }
    }

    void CheckNode()
    {
        //smoothing go the farther visible node of the path (to keep?)
        
        int i = 0;
        

        foreach (TileNode n in reversePath)
        {

            Debug.DrawRay(transform.position, (n.worldPosition - (Vector2)transform.position), Color.green, 2f);
            RaycastHit2D hit = Physics2D.Linecast(transform.position, n.worldPosition, LayerMask.GetMask("Unwalkable"));

            i++;

            if (i == (reversePath.Count - currentNode)) 
                break;

            //print(hit.collider.gameObject.name);
            //leave it 8D
            if (hit.collider == null)
            {
                //print("nodo" + n.description);
                currentPositionHolder = n.worldPosition;
                //Debug.Log(i+" - "+currentPositionHolder);
               
                //break;
            }
            
          
           
        }

    }

    public void PathToNextAction(Transform target)
    {

        path = new List<TileNode>();

        path = GetPath(transform, target);
        
        tileGrid.GetComponent<TileGridGenerator>().SetGizmosPath(path);
        reversePath = path;
        reversePath.Reverse();
        if (path.Count > 0)
        {
            currentPositionHolder = path.ToArray()[0].worldPosition;
        }
            
        currentNode = 0;

        CheckNode();

        followPath = true;

    }

    public List<TileNode> GetPath(Transform start, Transform end)
    {
        List<TileNode> aStarPath;
        TileNode nodeStart = transform.parent.GetComponent<TileGridGenerator>().NodeFromWorldPoint(start.position);
        TileNode nodeTarget = transform.parent.GetComponent<TileGridGenerator>().NodeFromWorldPoint(end.position);
        gridSizeX = transform.parent.GetComponent<TileGridGenerator>().GetSizeX();
        gridSizeY = transform.parent.GetComponent<TileGridGenerator>().GetSizeY();
        aStarPath = AStarSolverGrid.Solve(transform.parent.GetComponent<TileGridGenerator>().GetGrid(), gridSizeX, gridSizeY, nodeStart, nodeTarget, EuclideanEstimator);
        return aStarPath;
    }

    float EuclideanEstimator(TileNode from, TileNode to)
    {
        return (from.worldPosition - to.worldPosition).magnitude;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.gameObject.tag == "Wardrobe" || collision.gameObject.tag == "Workstation" || collision.gameObject.tag == "Fridge"
            || collision.gameObject.tag == "Pot" || collision.gameObject.tag == "Delivery")
        {
            Debug.Log("Collisione"+ collision.gameObject.tag);
            if (collision.gameObject.tag == "Delivery")
            {
                collision.gameObject.transform.parent.GetChild(1).gameObject.SetActive(true);
                
               // Debug.Log("collisione");
            }
                
            transform.GetComponent<PersonalityAgent>().targetReached = true;
            followPath = false;
        }

    }


}