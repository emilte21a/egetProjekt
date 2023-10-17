using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteRenderer spriteRenderer;
    public Animator animator;

    [SerializeField]
    public float speed = 5;

    [SerializeField]
    public float jump = 400;
    public Rigidbody2D rb;
    private bool facingRight = true;
    public bool isJumping;

    float groundRadius= 0.2f;

    [SerializeField] 
    Transform feet;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        rb.velocity = new Vector2(moveHorizontal * speed, rb.velocity.y);

        if (moveHorizontal != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isJumping == false)
        {
            rb.AddForce(new Vector2(rb.velocity.x, jump));
        }

        Debug.Log(isJumping);
        if (rb.velocity.x < 0)
        {
            facingRight = false;
        }
        else if (rb.velocity.x > 0)
        {
            facingRight = true;
        }

        if (facingRight == true)
        {
            transform.eulerAngles = new Vector3(0, 0, 0); // normal
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0); // flipped
        }


        // Try out this delta time method??
        //rb2d.transform.position += new Vector3(speed * Time.deltaTime, 0.0f, 0.0f);
    }

    
    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.CompareTag("World"))
        {
            isJumping = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collider)
    {
        if (collider.gameObject.CompareTag("World"))
        {
            isJumping = true;
        }
    }
    
}
