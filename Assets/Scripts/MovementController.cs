using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public ParticleSystem dust;
    public Animator animator;

    [Header("Movement Controls")]
    public float speed = 5;

    public float jumpForce = 400;

    public Rigidbody2D rb;

    private bool facingRight;

    private bool canJump;


    [Header("Ground Check")]
    public float castDistance = 1;
    public Vector2 boxSize;
    public LayerMask layerMask;


    [SerializeField]
    Transform feet;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        facingRight = true;
    }

    private void Update()
    {
        isGrounded();
       
        transform.eulerAngles = new Vector3(0, 0, 0);

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
            Jump();


        if (rb.velocity.x < 0)
            facingRight = false;

        else if (rb.velocity.x > 0)
            facingRight = true;


        if (facingRight)
            transform.eulerAngles = new Vector3(0, 0, 0); // normal

        else
            transform.eulerAngles = new Vector3(0, 180, 0); // flipped

    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        rb.velocity = new Vector2(moveHorizontal * speed, rb.velocity.y);


        if (moveHorizontal != 0)
        {
            animator.SetBool("isRunning", true);
            CreateDust();
        }

        else
            animator.SetBool("isRunning", false);
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void isGrounded()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, layerMask))
            canJump = true;

        else
        {
            CreateDust();
            StartCoroutine(CoyoteTimer());
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }

    private void CreateDust(){
        dust.Play();
    }

    IEnumerator CoyoteTimer()
    {
        yield return new WaitForSecondsRealtime(1f);
        canJump = false;
        
    }

}
