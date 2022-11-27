using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    private Rigidbody rb;
    private float limitXRight = 0.0f;
    private float limitXLeft = 0.0f;
    [SerializeField]private float speed = 4.0f;
    [SerializeField]private float offsetX = 2.0f;
    SpriteRenderer sprite;
    private bool goLeft = false;
    private bool goRight = false;
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sprite = GetComponent<SpriteRenderer>();
        limitXRight = rb.transform.position.x + offsetX;
        limitXLeft = rb.transform.position.x - offsetX;
        goRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        Vector2 velocity = new Vector2(0.0f,0.0f);
        if (goRight)
            velocity.x += speed;

        else if (goLeft) velocity.x -= speed;

        if(rb.transform.position.x > limitXRight)
        {
            sprite.flipX = true;
            goLeft = true;
            goRight = false;
        }

        if(rb.transform.position.x < limitXLeft)
        {
            sprite.flipX = false;
            goLeft = false;
            goRight = true;
        }

        rb.velocity = velocity;
    }
}
