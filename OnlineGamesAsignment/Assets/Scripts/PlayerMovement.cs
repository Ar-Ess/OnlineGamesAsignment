using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Placement")]
    [SerializeField] private Transform spawnpoint;

    [Header("Physics")]
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [Header("Time")]
    [SerializeField] float timeInterval = 1.0f;

    // Private
    private StreamFlag flag = new StreamFlag(0);
    private bool ground = false;
    private Rigidbody rb;
    private Collider collider;
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
        collider = GetComponent<Collider>();
    }

    private void Update()
    {
        flag.Clear();
        UpdateLogic();
        //UpdateWorldCheck();
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
        Vector2 velocity = new Vector2();

        velocity += Movement();
        velocity += Jump();

        UpdateAnimations(velocity);

        rb.velocity = new Vector2(velocity.x, rb.velocity.y + velocity.y );
    }

    private Vector2 Movement()
    {
        Vector2 velocity = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.A)) velocity.x -= speed;

        if (Input.GetKey(KeyCode.D)) velocity.x += speed;

        if (Input.GetKeyUp(KeyCode.A) ||Input.GetKeyUp(KeyCode.D)) flag.Set(3, true);

        return velocity;
    }

    private Vector2 Jump()
    {
        Vector2 velocity = new Vector2(0, 0);

        if (ground && Input.GetKeyDown(KeyCode.Space))
        {
            ground = false;
            flag.Set(2, true);
            velocity.y += jumpForce;
        }

        return velocity;
    }

    private void UpdateAnimations(Vector2 velocity)
    {
        if (velocity.x > 0 && velocity.y <= 0.1 && ground)
        {
            sprite.flipX = false;
            anim.SetInteger("Animation", 1);
            flag.Set(0, true);
        }
        else if (velocity.x < 0 && velocity.y <= 0.1 && ground)
        {
            sprite.flipX = true;
            anim.SetInteger("Animation", 1);
            flag.Set(1, true);
        }
        else if (velocity.x == 0)
        {
            anim.SetInteger("Animation", 0);
        }
        else if (velocity.y > 0 && !ground)
        {
            anim.SetInteger("Animation", 2);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("OnlinePlayer"))
            Physics.IgnoreCollision(collision.collider, collider);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!ground && collision.collider.tag.Equals("Ground"))
            ground = true;
    }
}
