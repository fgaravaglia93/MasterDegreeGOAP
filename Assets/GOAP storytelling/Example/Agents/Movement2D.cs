using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem.Runtime;

public class Movement2D : MonoBehaviour
{
    public float speed = 100;
    Vector2 direction;
    private float x, y;
    private bool isWalking = false;
    private Animator animator;
    private Rigidbody2D rb;
   

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>() ;
    }

    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        direction = Vector2.zero;

        if (x != 0f || y != 0f)
        {
            if (Mathf.Abs(x) >= Mathf.Abs(y))
                y = 0f;
            else
                x = 0f;

            isWalking = true;
            animator.SetBool("isWalking", isWalking);
            Move();
        }
        else
        {
            if (isWalking)
            {
                isWalking = false;
                animator.SetBool("isWalking", isWalking);
            }
        }

    }

    void Move()
    {
            animator.SetFloat("X", x);
            animator.SetFloat("Y", y);
            if (Mathf.Abs(x) >= Mathf.Abs(y))
            {
                rb.velocity = new Vector2(x * speed * 100 * Time.deltaTime, 0);
            }
            else
            {
                rb.velocity= new Vector2(0, y* speed * 100 * Time.deltaTime);
            }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Interact")
        {
            
            collision.gameObject.transform.parent.GetComponent<DialogueParser>().interactable = true;
            //collision.gameObject.GetComponent<DialogueParser>().enabled = true;
        }        
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Interact")
        {
            collision.gameObject.transform.parent.GetComponent<DialogueParser>().interactable = false;
            //collision.gameObject.GetComponent<DialogueParser>().enabled = true;
        }
    }
}
