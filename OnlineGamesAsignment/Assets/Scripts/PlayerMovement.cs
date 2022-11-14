using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform spawnpoint;
    SpriteRenderer sprite;
    Animator anim;

    [Header("Physics")]
    [SerializeField] float speed = 50;
    [SerializeField] float acceleration = 1;
    [SerializeField] float jumpForce = 50;

    private StreamFlag flag = new StreamFlag(0);
    private MemoryStream stream = new MemoryStream();
    bool ground = false;
    Rigidbody rb;

    public Serializer serializer = new Serializer();

    private void Awake()
    {
        transform.position = spawnpoint.position;
    }

    public bool IsAnyInputActive()
    {
        return flag.IsAnyTrue();
    }

    public uint GetFlag()
    {
        return flag.flag;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        flag.Clear();
        PlayerMove();
    }

    private void PlayerMove()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        if (rb.velocity.x > 0 && rb.velocity.y <= 0.1 && ground)
        {
            sprite.flipX = false;
            anim.SetInteger("Animation", 1);
            flag.Set(0, true);
        }

        else if (rb.velocity.x < 0 && rb.velocity.y <= 0.1 && ground)
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

        if (Input.GetKey(KeyCode.Space) && ground)
        {
            flag.Set(2, true);
            Jump();
        }
    }

    private void Jump() => rb.velocity = new Vector2(0, jumpForce);

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Ground")
            ground = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Ground")
            ground = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "OnlinePlayer")
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), collision.collider);
        }
    }
}
