using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] Camera camera;
    [SerializeField] Transform spawnpoint;
    SpriteRenderer sprite;
    Animator anim;

    [Header("Physics")]
    [SerializeField] float speed = 50;
    [SerializeField] float acceleration = 1;
    [SerializeField] float jumpForce = 50;

    
    public MemoryStream stream = new MemoryStream();

    float gravityScale = 10;
    bool ground = false;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        PlayerMove();
        if (Input.GetKey(KeyCode.Space) && ground) Jump();

        
    }

    private void PlayerMove()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        if (rb.velocity.x > 0 && rb.velocity.y <= 0.1 && ground)
        {
            sprite.flipX = false;
            anim.SetInteger("Animation", 1);
        }
  
        else if (rb.velocity.x < 0 && rb.velocity.y <= 0.1 && ground)
        {
            sprite.flipX = true;
            anim.SetInteger("Animation", 1);
        }
        
        else if(rb.velocity.x == 0 )
        {
            anim.SetInteger("Animation", 0);
        }

        else if(rb.velocity.y > 0 && !ground)
        {
            anim.SetInteger("Animation", 2);
        }
    }

    private void Jump() => rb.velocity = new Vector2(0, jumpForce);

    private void OnCollisionStay(Collision collision)
    {
        if(collision.collider.tag == "Ground")
        ground = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Ground")
            ground = false;
    }
}
