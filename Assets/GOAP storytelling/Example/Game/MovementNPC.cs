using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementNPC : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
    Rigidbody2D rb;
    [HideInInspector]
    public bool firstInteract;
    [HideInInspector]
    public bool backtoMove;
    [HideInInspector]
    public Transform hero;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        firstInteract = false;
        backtoMove = false;
        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (firstInteract)
        {
            
            animator.SetBool("isWalking", true);
            //look at player
            MoveD4(DirectionD4(hero.position), 0);
            //Debug.Log(hero.position);
            firstInteract = false;
            animator.SetBool("isWalking", false);
        }

        if (backtoMove&& GetComponent<MoveToNextAction>()!=null)
        {
            animator.SetBool("isWalking", true);
            //look at player
            MoveD4(GetComponent<MoveToNextAction>().direction, 0);
            backtoMove = false;
        }
    }

    private void FixedUpdate()
    {
        if (!animator.GetBool("isWalking"))
            rb.velocity = Vector2.zero;
    }

    public Vector2 GetHeroDirection()
    {
        return DirectionD4(hero.position);
    }

    public Vector2 GetTargetDirection(Vector2 target)
    {
        return DirectionD4(target);
    }


    Vector2 DirectionD4(Vector2 target)
    {
        var dirX = (target.x - transform.position.x + .1f) / Mathf.Abs(target.x - transform.position.x + .1f);
        var dirY = (target.y - transform.position.y + .1f) / Mathf.Abs(target.y - transform.position.y + .1f);
        //Debug.Log(Mathf.Abs(currentPositionHolder.x - transform.position.x) + " , " + Mathf.Abs(currentPositionHolder.y - transform.position.y));
        if (Mathf.Abs(target.x - transform.position.x) >= Mathf.Abs(target.y - transform.position.y))
        {
            animator.SetFloat("X", dirX);
            animator.SetFloat("Y", 0);
            return new Vector2(dirX * 1, 0);
        }
        else
        {
            animator.SetFloat("Y", dirY);
            animator.SetFloat("X", 0);
            return new Vector2(0, dirY * 1);
        }
    }

    public void MoveD4(Vector2 direction, float speedDir)
    {
        if (direction.y == 0)
            rb.velocity = new Vector2(direction.x * speedDir * 100 * Time.deltaTime, 0);
        else
            rb.velocity = new Vector2(0, direction.y * speedDir * 100 * Time.deltaTime);
    }
}
