using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlinePlayerMovement : MonoBehaviour
{
    [SerializeField] Transform spawnpoint;
    SpriteRenderer sprite;
    Animator anim;

    bool receiveInputs = false;
    StreamFlag flag = new StreamFlag(0);

    [Header("Physics")]
    [SerializeField] float speed = 50;
    [SerializeField] float acceleration = 1;
    [SerializeField] float jumpForce = 50;

    Rigidbody rb;
    Collider collider;

    private void Awake()
    {
        transform.position = spawnpoint.position;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (receiveInputs) MovementLogic();
    }

    public void SetFlag(uint uflag)
    {
        flag = new StreamFlag(uflag);
        receiveInputs = true;
    }

    private void MovementLogic()
    {
        if (!flag.IsAnyTrue())
        {
            anim.SetInteger("Animation", 0);
            receiveInputs = false;
            return;
        }

        if (flag.Get(0)) MoveRight();
        if (flag.Get(1)) MoveLeft();
        if (flag.Get(2)) Jump();

        receiveInputs = false;
    }

    private void MoveLeft()
    {
        rb.velocity = new Vector2(-speed, rb.velocity.y);
        sprite.flipX = true;
        anim.SetInteger("Animation", 1);
    }
    
    private void MoveRight()
    {
        rb.velocity = new Vector2(speed, rb.velocity.y);
        sprite.flipX = false;
        anim.SetInteger("Animation", 1);
    }

    private void Jump() 
    { 
        rb.velocity = new Vector2(0, jumpForce);
        anim.SetInteger("Animation", 2);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "LocalPlayer")
        {
            Physics.IgnoreCollision(collision.collider, collider);
        }
    }
}
