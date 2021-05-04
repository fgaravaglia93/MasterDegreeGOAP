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
    public float speed = 20;
    private int currentNode;

    public Transform target;
    public GameObject placeholder;
    [HideInInspector]
    public Vector2 direction;
    [HideInInspector]
    public bool firstInteract;

    // Use this for initialization
    void Start()
    {
        grid = transform.parent.GetComponentInChildren<TileGridGenerator>().GetGrid();
       
    }

    private void Update()
    {

        if (followPath)
        {
               // transform.position = Vector3.Lerp(transform.position, transform.position, Time.deltaTime * speed);

            if (!GetComponent<PersonalityAgent>().interaction)
            {
                if ((transform.position - currentPositionHolder).magnitude > 0.2f)
                {
                    GetComponent<MovementNPC>().animator.SetBool("isWalking", true);
                    GetComponent<MovementNPC>().MoveD4(direction, speed);
                }
                else
                {
                    GetComponent<MovementNPC>().animator.SetBool("isWalking", false);
                    if (currentNode < path.Count)
                    {
                        //Debug.Log("check");
                        CheckNode();
                        currentNode++;
                    }
                }
            }
            else
            {
            }
                
        }
    }

   /* private void FixedUpdate()
    {
        if(!animator.GetBool("isWalking"))
            rb.velocity = Vector2.zero;
    }*/

    void CheckNode()
    {
        if(currentNode < path.ToArray().Length)
        {
            TileNode n = path.ToArray()[currentNode];
            if (currentNode > 0)
                transform.position = path.ToArray()[currentNode - 1].worldPosition;

            Debug.DrawRay(transform.position, (n.worldPosition - (Vector2)transform.position), Color.green, 2f);
            RaycastHit2D hit = Physics2D.Linecast(transform.position, n.worldPosition, LayerMask.GetMask("Unwalkable"));
            currentPositionHolder = n.worldPosition;
            direction = GetComponent<MovementNPC>().GetTargetDirection(currentPositionHolder);
        }
    }
        


    public void PathToNextAction(Transform target)
    {
        this.target = target;
        path = new List<TileNode>();

        path = GetPath(transform, target);
        
        tileGrid.GetComponent<TileGridGenerator>().SetGizmosPath(path);

        reversePath = path;
        //reversePath.Reverse();
        if (path.Count > 0)
        {
            currentPositionHolder = path.ToArray()[0].worldPosition;
        }
            
        currentNode = 0;

        TileNode start = transform.parent.GetComponentInChildren<TileGridGenerator>().NodeFromWorldPoint(transform.position);
        transform.parent.GetComponentInChildren<TileGridGenerator>().check = start;

        CheckNode();

        followPath = true;

    }

    public List<TileNode> GetPath(Transform start, Transform end)
    {
        List<TileNode> aStarPath;
        TileNode nodeStart = transform.parent.GetComponentInChildren<TileGridGenerator>().NodeFromWorldPoint(start.position - (new Vector3(0,1,0)));
        TileNode nodeTarget = transform.parent.GetComponentInChildren<TileGridGenerator>().NodeFromWorldPoint(end.position);
        gridSizeX = transform.parent.GetComponentInChildren<TileGridGenerator>().GetSizeX();
        gridSizeY = transform.parent.GetComponentInChildren<TileGridGenerator>().GetSizeY();
        aStarPath = AStarSolverGrid.Solve(transform.parent.GetComponentInChildren<TileGridGenerator>().GetGrid(), gridSizeX, gridSizeY, nodeStart, nodeTarget, EuclideanEstimator);
        return aStarPath;
    }

    float EuclideanEstimator(TileNode from, TileNode to)
    {
        return (from.worldPosition - to.worldPosition).magnitude;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.gameObject == target.gameObject){
            //Debug.Log("Collisione"+ collision.gameObject.tag);
            if (collision.gameObject.tag == "Delivery")
            {
                collision.gameObject.transform.parent.GetChild(1).gameObject.SetActive(true);
               // Debug.Log("collisione");
            }
                
            transform.GetComponent<HogwartsStudent>().targetReached = true;
            followPath = false;
            //Set idle
            GetComponent<MovementNPC>().animator.SetBool("isWalking", false);

        }

    }


}