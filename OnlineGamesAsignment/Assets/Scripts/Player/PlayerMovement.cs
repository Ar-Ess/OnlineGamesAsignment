using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Physics")]
    [SerializeField] float speed = 0;
    [SerializeField] float jumpForce = 0;
    [Header("World Check")]
    [SerializeField] float timeIntervalWorldCheck = 1.0f;

    // Accessors
    public uint Flag { get { return flag.flag; } }
    public bool IsAnyInputActive { get { return flag.IsAnyTrue(); } }
    public Vector2 WorldCheck { get { return worldCheckVector.GetValueOrDefault(); } }
    public bool IsSendWorldCheck { get { return (worldCheckVector != null); } }

    public int Health { get { return health; } }
    public int MaxHealth { get { return maxHealth; } }

    // Private
    private StreamFlag flag = new StreamFlag(0);
    private bool ground = false;
    private Rigidbody rb;
    private Collider playerCollider;
    private SpriteRenderer sprite;
    private Animator anim;
    private float timer = 0;
    private Vector2? worldCheckVector = null;
    private Vector2 spawn = new Vector2(0.0f,0.0f);
    private int health;
    private int maxHealth;

    private void Awake()
    {
        spawn = transform.position;
        health = 10;
        maxHealth = health;
    }

    private void Start()
    {
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

        if (timer < timeIntervalWorldCheck) timer += Time.deltaTime;
        else worldCheckVector = new Vector2(transform.position.x, transform.position.y);
    }

    public void ClearWorldCheckVector()
    {
        worldCheckVector = null;
        timer = 0.0f;
    }

    public void ClearInputFlag()
    {
        flag.Clear();
    }

    private void UpdateLogic()
    {
        StreamFlag frameFlag = new StreamFlag(0);

        PlayerMove(frameFlag);
        Jump(frameFlag);
        UpdateAnimations();

        flag.Set(frameFlag.flag);
    }

    private void PlayerMove(StreamFlag frameFlag)
    {
        Vector2 velocity = new Vector2(0, rb.velocity.y);
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            frameFlag.Set(0, true);
            velocity.x += speed;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            frameFlag.Set(1, true);
            velocity.x -= speed;
        }

        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow) ||
            Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            frameFlag.Set(3, true);
            velocity.x = 0;
        }

        rb.velocity = velocity;
    }

    private void Jump(StreamFlag frameFlag)
    {
        if (ground && Input.GetKeyDown(KeyCode.Space))
        {
            frameFlag.Set(2, true);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void UpdateAnimations()
    {
        // IF ON GROUND
        if (rb.velocity.y <= 0.3f && ground)
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

    private void Death()
    {
        transform.position = spawn;
    }

    private void Hit()
    {
        health -= 1;
        Debug.Log("Hit");

        if (health <= 0)
        {
            Death();
            health = maxHealth;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("OnlinePlayer"))
            Physics.IgnoreCollision(collision.collider, playerCollider);
        if (collision.gameObject.tag.Equals("Death"))
            Death();
        if(collision.gameObject.tag.Equals("Enemy"))
        {
            Hit();
        }

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
