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

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 direction;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        grid = transform.parent.GetComponent<TileGridGenerator>().GetGrid();
        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 0);

    }

    private void Update()
    {

        if (followPath)
        {

            if ((transform.position - currentPositionHolder).magnitude > 0.2f)
            {
                animator.SetBool("isWalking", true);

               // transform.position = Vector3.Lerp(
                 //   new Vector3(transform.position.x, transform.position.y, 0f), new Vector3(currentPositionHolder.x, 0f, 0f), Time.deltaTime * speed);
                MoveD4(direction);
            }
            else
            {
                if (currentNode < path.Count)
                {
                    Debug.Log("check");
                    CheckNode();
                    currentNode++;
                }
            }
        }
    }

    void CheckNode()
    {
        
        TileNode n = path.ToArray()[currentNode];
        if(currentNode > 0)
            transform.position = path.ToArray()[currentNode-1].worldPosition;
        Debug.DrawRay(transform.position, (n.worldPosition - (Vector2)transform.position), Color.green, 2f);
        RaycastHit2D hit = Physics2D.Linecast(transform.position, n.worldPosition, LayerMask.GetMask("Unwalkable"));
        currentPositionHolder = n.worldPosition;
        //Instantiate(placeholder, new Vector3(currentPositionHolder.x, currentPositionHolder.y, 0f), Quaternion.identity);
        var dirX = (currentPositionHolder.x - transform.position.x + .1f) / Mathf.Abs(currentPositionHolder.x - transform.position.x + .1f);
        var dirY = (currentPositionHolder.y - transform.position.y + .1f) / Mathf.Abs(currentPositionHolder.y - transform.position.y + .1f);
        Debug.Log(currentPositionHolder);
        Debug.Log(Mathf.Abs(currentPositionHolder.x - transform.position.x) + " , " + Mathf.Abs(currentPositionHolder.y - transform.position.y));
        if (Mathf.Abs(currentPositionHolder.x - transform.position.x) >= Mathf.Abs(currentPositionHolder.y - transform.position.y))
        {
            animator.SetFloat("X", dirX);
            animator.SetFloat("Y", 0);
            direction = new Vector2(dirX * 1, 0);

        }
        else
        {
            animator.SetFloat("Y", dirY);
            animator.SetFloat("X", 0);
            direction = new Vector2(0, dirY * 1);
        }

        /*      int i = 0;
                foreach (TileNode n in reversePath)
                {
                    Debug.DrawRay(transform.position, (n.worldPosition - (Vector2)transform.position), Color.green, 2f);
                    RaycastHit2D hit = Physics2D.Linecast(transform.position, n.worldPosition, LayerMask.GetMask("Unwalkable"));
                    i++;
                    if (i == (reversePath.Count - currentNode)) 
                        break;
                    if (hit.collider == null)
                    {
                        //print("nodo" + n.description);
                        currentPositionHolder = n.worldPosition;
                        Instantiate(placeholder, new Vector3(currentPositionHolder.x,currentPositionHolder.y,0f), Quaternion.identity);
                        var dirX = (currentPositionHolder.x - transform.position.x)/ Mathf.Abs(currentPositionHolder.x - transform.position.x);
                        var dirY = (currentPositionHolder.y - transform.position.y) / Mathf.Abs(currentPositionHolder.y - transform.position.y);
                        animator.SetFloat("X", dirX);
                        animator.SetFloat("Y", dirY);
                        Debug.Log(Mathf.Abs(currentPositionHolder.x - transform.position.x) + " , " + Mathf.Abs(currentPositionHolder.y - transform.position.y));
                        if (Mathf.Abs(currentPositionHolder.x - transform.position.x) >= Mathf.Abs(currentPositionHolder.y - transform.position.y))
                            direction = new Vector2(dirX * 10, 0);
                        else
                            direction = new Vector2(0, dirY * 10);
                        //smoothing go the farther visible node of the path (8D)
                        //break;
                    }
                }*/
    }

    public void PathToNextAction(Transform target)
    {

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

        TileNode start = transform.parent.GetComponent<TileGridGenerator>().NodeFromWorldPoint(transform.position);
        transform.parent.GetComponent<TileGridGenerator>().check = start;

        CheckNode();

        followPath = true;

    }

    public List<TileNode> GetPath(Transform start, Transform end)
    {
        List<TileNode> aStarPath;
        TileNode nodeStart = transform.parent.GetComponent<TileGridGenerator>().NodeFromWorldPoint(start.position - (new Vector3(0,1,0)));
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
            || collision.gameObject.tag == "Pot" || collision.gameObject.tag == "Delivery" || collision.gameObject.tag == "Book"
            || collision.gameObject.tag == "Helper")
        {
            //Debug.Log("Collisione"+ collision.gameObject.tag);
            if (collision.gameObject.tag == "Delivery")
            {
                collision.gameObject.transform.parent.GetChild(1).gameObject.SetActive(true);
                
               // Debug.Log("collisione");
            }
                
            transform.GetComponent<HogwartsStudent>().targetReached = true;
            followPath = false;
            //Set idle
            animator.SetBool("isWalking", false);

        }

    }

    void MoveD4(Vector2 direction)
    {
        //animator.SetFloat("X", (direction.x > 0) ? 1 : 0);
        //animator.SetFloat("Y", (direction.y > 0) ? 1 : 0);
        //rb.velocity = direction * speed * 10 * Time.deltaTime;
        if(direction.y == 0)
        transform.position = Vector3.Lerp(
                 new Vector3(transform.position.x, currentPositionHolder.y, 0f), 
                 new Vector3(currentPositionHolder.x, currentPositionHolder.y, 0f), Time.deltaTime * speed);
        else
            transform.position = Vector3.Lerp(
                     new Vector3(transform.position.x, currentPositionHolder.y, 0f),
                     new Vector3(currentPositionHolder.x, currentPositionHolder.y, 0f), Time.deltaTime * speed);
        

    }


}