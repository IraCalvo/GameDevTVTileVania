using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider;
    BoxCollider2D feetCollider;
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float climbSpeed;
    [SerializeField] Vector2 deathKick = new Vector2();
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawn;
    Animator playerAnimator;
    float gravityScaleAtStart;

    bool isAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();    
        playerAnimator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(isAlive)
        {
            Run();
            FlipSprite();
            ClimbLadder();
            Die();
        }
    }

    void OnFire(InputValue value)
    {
        if(isAlive)
        {
            playerAnimator.SetTrigger("Shooting");
            Instantiate(projectile, projectileSpawn);
        }
    }

    void OnMove(InputValue value)
    {
        if(isAlive)
        {
            moveInput = value.Get<Vector2>();
            Debug.Log(moveInput);
        }
        
    }

    void OnJump(InputValue value)
    {
        if(isAlive)
        {
            if(value.isPressed && feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                rb.velocity += new Vector2(0f, jumpSpeed);
            }
        }
    }

    void Run()
    {

        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        playerAnimator.SetBool("isRunning", playerHasHorizontalSpeed);

    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;

        if(playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
        }
    }
    
    void ClimbLadder()
    {
        if(!feetCollider.IsTouchingLayers(LayerMask.GetMask("Climbable")))
        {
            rb.gravityScale = gravityScaleAtStart;
            playerAnimator.SetBool("isClimbing", false);
            return;
        }

        Vector2 climbVelocity = new Vector2(rb.velocity.x, moveInput.y * climbSpeed);
        bool playerHasVerticalSpeed = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;
        rb.velocity = climbVelocity;
        rb.gravityScale = 0f;
        playerAnimator.SetBool("isClimbing", playerHasVerticalSpeed);

    }

    void Die()
    {
        if(rb.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            playerAnimator.SetTrigger("Dying");
            rb.velocity = deathKick;
            isAlive = false;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

}
