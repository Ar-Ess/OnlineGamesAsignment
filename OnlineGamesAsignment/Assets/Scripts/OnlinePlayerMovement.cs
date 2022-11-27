﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlinePlayerMovement : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] float speed = 0;
    [SerializeField] float jumpForce = 0;

    // Private
    private StreamFlag flag = new StreamFlag(0);
    private bool receiveInputs = false;
    private Rigidbody rb;
    private Collider playerCollider;
    private SpriteRenderer sprite;
    private Animator anim;
    private Vector2? receivePosition = null;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if(receiveInputs) UpdateLogic();

        if(receivePosition != null)
        {
            transform.position = new Vector3(receivePosition.GetValueOrDefault().x, receivePosition.GetValueOrDefault().y, transform.position.z);
            receivePosition = null;
        }
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
            anim.SetInteger("Animation", 0);
            receiveInputs = false;
            return;
        }

        Vector2 velocity = new Vector2(0,0);

        if (flag.Get(0)) velocity += MoveRight();
        if (flag.Get(1)) velocity += MoveLeft();
        if (flag.Get(2)) velocity += Jump();
        if (flag.Get(3)) rb.velocity = new Vector2(0.0f, rb.velocity.y);

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
        Debug.Log("jump");
        anim.SetInteger("Animation", 2);
        return new Vector2(0, jumpForce);
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
