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
        if (ground && Input.GetKeyDown(KeyCode.Space)) Jump();

        UpdateAnimations(); 
    }

    private void PlayerMove()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
    }

    private void Jump()
    {

        if (ground && Input.GetKeyDown(KeyCode.Space))
        {
            flag.Set(2, true);
            rb.velocity = new Vector2(0, jumpForce);
        }
   
    }

    private void UpdateAnimations()
    {
        if (rb.velocity.x > 0 && rb.velocity.y <= 0.1f && ground)
        {
            sprite.flipX = false;
            anim.SetInteger("Animation", 1);
            flag.Set(0, true);
        }
        else if (rb.velocity.x < 0 && rb.velocity.y <= 0.1f && ground)
        {
            sprite.flipX = true;
            anim.SetInteger("Animation", 1);
            flag.Set(1, true);
        }
        else if (rb.velocity.x == 0)
        {
            anim.SetInteger("Animation", 0);
        }
        else if (rb.velocity.y > 0 && !ground)
        {
            anim.SetInteger("Animation", 2);
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
