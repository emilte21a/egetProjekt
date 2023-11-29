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
        //Kollar om spelaren nuddar marken
        isGrounded(); 

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            CreateDust();
            Jump();
        }

        //Om spelaren rör sig åt vänster så är facingRight falsk
        if (Input.GetAxisRaw("Horizontal") < 0)
            facingRight = false;

        //Annars om spelaren rör sig åt höger så är facingRight sann
        else if (Input.GetAxisRaw("Horizontal") > 0)
            facingRight = true;

        //Om spelaren är riktad åt höger så är dess y rotation normal
        if (facingRight)
            transform.eulerAngles = new Vector3(0, 0, 0); 


        //Annars så är den flippad 180 grader
        else
            transform.eulerAngles = new Vector3(0, 180, 0); 

    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        rb.velocity = new Vector2(moveHorizontal * speed, rb.velocity.y);

        //Om spelaren rör sig sätt isRunning till sann och sätt igång en partikel effekt
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

    //Metod som kollar om spelaren nuddar marken
    private void isGrounded()
    {
        //Om en boxcast med ett antal parametrar nuddar layermasken(marken) så är canJump sann
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, layerMask))
            canJump = true;

        //Annars så ska en coRoutine startas
        else
            StartCoroutine(CoyoteTimer());   
    }

    //Metod som ritar ut boxcasten i tidigare metod 
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }

    private void CreateDust(){
        dust.Play();
    }   

    //En IEnumerator som väntar i 0.2 sekunder innan canJump blir falsk vilket gör att movementen i spelet känns mer responsivt
    IEnumerator CoyoteTimer()
    {
        yield return new WaitForSeconds(0.2f);
        canJump = false;
        
    }

}
