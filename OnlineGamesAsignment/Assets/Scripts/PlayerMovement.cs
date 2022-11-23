using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Placement")]
    [SerializeField] private Transform spawnpoint = null;

    [Header("Physics")]
    [SerializeField] float speed = 0;
    [SerializeField] float jumpForce = 0;
    [Header("Time")]
    [SerializeField] float timeInterval = 1.0f;

    // Private
    private StreamFlag flag = new StreamFlag(0);
    private bool ground = false;
    private Rigidbody rb;
    private Collider playerCollider;
    private SpriteRenderer sprite;
    private Animator anim;
    private float timer = 0;
    private Vector2? worldCheckVector = null;

    private void Start()
    {
        transform.position = spawnpoint.position;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        flag.Clear();
        UpdateLogic();
        UpdateWorldCheck();
    }

    private void UpdateWorldCheck()
    {
        if (worldCheckVector != null) return;

        if (timer < timeInterval) timer += Time.deltaTime;
        else worldCheckVector = new Vector2(transform.position.x, transform.position.y);
    }

    public bool IsSendingWorldCheck()
    {
        return (worldCheckVector != null);
    }

    public void ClearWorldCheckVector()
    {
        worldCheckVector = null;
        timer = 0.0f;
    }

    public Vector2? GetWorldCheck()
    {
        return worldCheckVector;
    }

    public bool IsAnyInputActive()
    {
        return flag.IsAnyTrue();
    }

    public uint GetFlag()
    {
        return flag.flag;
    }

    public void ClearFlag()
    {
        flag.Clear();
    }

    private void UpdateLogic()
    {
        PlayerMove();
        Jump();
        UpdateAnimations(); 
    }

    private void PlayerMove()
    {
        Vector2 velocity = new Vector2(0, rb.velocity.y);
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            flag.Set(0, true);
            velocity.x += speed;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            flag.Set(1, true);
            velocity.x -= speed;
        }

        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow) ||
            Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            flag.Set(3, true);
            velocity.x = 0;
        }

        rb.velocity = velocity;
    }

    private void Jump()
    {
        if (ground && Input.GetKeyDown(KeyCode.Space))
        {
            flag.Set(2, true);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void UpdateAnimations()
    {
        // IF ON GROUND
        if (rb.velocity.y <= 0.1f && ground)
        {
            // IF GOIN RIGHT ON GROUND
            if (rb.velocity.x > 0)
            {
                sprite.flipX = false;
                anim.SetInteger("Animation", 1);
            }
            // IS GOING LEFT ON GROUND
            else if (rb.velocity.x < 0)
            {
                sprite.flipX = true;
                anim.SetInteger("Animation", 1);
            }
            // IF STAY STILL
            else
            {
                anim.SetInteger("Animation", 0);
            }
        }
        else
        {
            anim.SetInteger("Animation", 2);
            // IF GOIN RIGHT ON AIR
            if (rb.velocity.x > 0) sprite.flipX = false;
            // IS GOING LEFT ON GROUND
            else if (rb.velocity.x < 0) sprite.flipX = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag.Equals("OnlinePlayer"))
            Physics.IgnoreCollision(collision.collider, playerCollider);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag.Equals("Ground"))
            ground = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Ground")
            ground = false;
    }
}
