using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlinePlayerMovement : MonoBehaviour
{

    [Header("Physics")]
    [SerializeField] float speed = 0;
    [SerializeField] float jumpForce = 0;
    [Header("World Check")]
    [SerializeField] int wCFreedomDegree = 1;
    [Header("Health")]
    [SerializeField] private GameObject healthBarInstance = null;

    // Private
    private StreamFlag flag = new StreamFlag(0);
    private bool receiveInputs = false;
    private Rigidbody rb;
    private Collider playerCollider;
    private SpriteRenderer sprite;
    private Animator anim;
    private Vector2? receivePosition = null;
    private HealthBar healthBar;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider>();

        GameObject healthBarObject = Instantiate(healthBarInstance);
        healthBar = healthBarObject.GetComponent<HealthBar>();
    }

    private void Update()
    {
        if(receiveInputs) UpdateLogic();

        if (receivePosition != null) UpdateWorldCheck();
    }

    private void UpdateWorldCheck()
    {
        Vector2 worldCheck = receivePosition.GetValueOrDefault();

        if (Mathf.Abs(transform.position.x - worldCheck.x) <= wCFreedomDegree) worldCheck.x = transform.position.x;
        if (Mathf.Abs(transform.position.y - worldCheck.y) <= wCFreedomDegree) worldCheck.y = transform.position.y;

        transform.position = new Vector3(worldCheck.x, worldCheck.y, transform.position.z);
        receivePosition = null;
    }

    public void SetFlag(uint uflag)
    {
        flag = new StreamFlag(uflag);
        receiveInputs = true;
    }

    private void UpdateLogic()
    {
        if (!flag.IsAnyTrue())
        {
            receiveInputs = false;
            return;
        }

        Vector2 velocity = new Vector2(0,0);

        if (flag.Get(0)) velocity += MoveRight();
        if (flag.Get(1)) velocity += MoveLeft();
        if (flag.Get(2)) velocity += Jump();
        if (flag.Get(3)) rb.velocity = Still();

        rb.velocity = new Vector2(velocity.x, rb.velocity.y + velocity.y);

        flag.Clear();
    }

    private Vector2 MoveLeft()
    {
        sprite.flipX = true;
        anim.SetInteger("Animation", 1);
        return new Vector2(-speed, 0);
    }
    
    private Vector2 MoveRight()
    {
        sprite.flipX = false;
        anim.SetInteger("Animation", 1);
        return new Vector2(speed, 0);
    }

    private Vector2 Jump() 
    {
        anim.SetInteger("Animation", 2);
        return new Vector2(0, jumpForce);
    }

    private Vector2 Still()
    {
        anim.SetInteger("Animation", 0);
        return new Vector2(0.0f, rb.velocity.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("LocalPlayer"))
            Physics.IgnoreCollision(collision.collider, playerCollider);
    }

    public void SetPosition(Vector2 vector2)
    {
        receivePosition = vector2;
    }
}
