using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlinePlayerMovement : MonoBehaviour
{
    [SerializeField] Transform spawnpoint;
    SpriteRenderer sprite;
    Animator anim;

    [Header("Physics")]
    [SerializeField] float speed = 50;
    [SerializeField] float acceleration = 1;
    [SerializeField] float jumpForce = 50;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void MovementLogic(uint uflag)
    {
        StreamFlag flag = new StreamFlag(uflag);
        if (!flag.IsAnyTrue())
        {
            anim.SetInteger("Animation", 0);
            return;
        }

        if (flag.Get(0)) MoveRight();
        if (flag.Get(1)) MoveLeft();
        if (flag.Get(2)) Jump();
    }

    private void MoveLeft()
    {
        rb.velocity = new Vector2(speed, rb.velocity.y);
        sprite.flipX = false;
        anim.SetInteger("Animation", 1);
    }
    
    private void MoveRight()
    {
        rb.velocity = new Vector2(-speed, rb.velocity.y);
        sprite.flipX = true;
        anim.SetInteger("Animation", 1);
    }

    private void Jump() 
    { 
        rb.velocity = new Vector2(0, jumpForce);
        anim.SetInteger("Animation", 2);
    }
}
