using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Properties")]
    public float speep = 2.5f;
    public float jumpForce = 2f;

    [Header("Ground Properties")]
    public LayerMask groundLayer;
    public float groundDistance;
    public bool isGrounded;
    public Vector3[] footOffset;

    private bool isJump;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private int direction = 1;
    private float originalXScale;
    private float xVelocity;
    private bool isFire;

    RaycastHit2D leftCheck;
    RaycastHit2D rightCheck;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalXScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        PhysicsCheck();

        if (!isFire)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            movement = new Vector2(horizontal, 0);

            if (xVelocity * direction < 0f)
                Flip();
        }

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (Input.GetButtonDown("Fire1") && !isFire && isGrounded)
        {
            movement = Vector2.zero;
            rb.velocity = Vector2.zero;
            animator.SetTrigger("fire");
        }
    }

    private void FixedUpdate()
    {
        if(!isFire)
        {
            xVelocity = movement.normalized.x * speep;
            rb.velocity = new Vector2(xVelocity, rb.velocity.y);
        }
    }

    private void LateUpdate()
    {
        //animator.SetTrigger("die");

        animator.SetFloat("xVelocity", Mathf.Abs(xVelocity));
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelcity", rb.velocity.y);

        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("fire"))
        {
            isFire = true;
        }
        else { isFire = false; }
    }

    private void PhysicsCheck()
    {
        isGrounded = false;
        leftCheck = Raycast(new Vector2(footOffset[0].x, footOffset[0].y), Vector2.down, groundDistance, groundLayer);
        rightCheck = Raycast(new Vector2(footOffset[1].x, footOffset[1].y), Vector2.down, groundDistance, groundLayer);

        if(leftCheck || rightCheck)
        {
            isGrounded = true;
        }
    }

    private void Flip()
    {
        direction *= -1;
        Vector3 scale = transform.localScale;

        scale.x = originalXScale * direction;
        transform.localScale = scale;
    }

    public RaycastHit2D Raycast(Vector3 origin, Vector2 rayDirection, float length, LayerMask mask)
    {
        Vector3 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(pos + origin, rayDirection, length, mask);


        Color color = hit ? Color.red : Color.green;

        Debug.DrawRay(pos + origin, rayDirection * length, color);


        return hit;
    }
}
