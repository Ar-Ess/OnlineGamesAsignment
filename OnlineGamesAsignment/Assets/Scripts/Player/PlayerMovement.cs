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
    [Header("Health")]
    [SerializeField] private GameObject healthBarInstance = null;
    [SerializeField] private int maxHealth = 5;
    [Header("Attack")]
    [SerializeField] private float attackRange = 0.13f;
    [SerializeField] private int damage = 5;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    // Accessors
    public uint Flag { get { return flag.flag; } }
    public bool IsAnyInputActive { get { return flag.IsAnyTrue(); } }
    public Vector2 WorldCheck { get { return worldCheckVector.GetValueOrDefault(); } }
    public bool IsSendWorldCheck { get { return (worldCheckVector != null); } }
    public int health { get { return healthBar.health; } set { healthBar.health = value; } }

    public bool NeedUpdateHealth { get { return states.Get(0); } set { states.Set(0, value); } }

    // Private
    private StreamFlag flag = new StreamFlag(0);
    // UpdateHealth(0)
    private StreamFlag states = new StreamFlag(0);
    private bool ground = false;
    private Rigidbody rb;
    private Collider playerCollider;
    private SpriteRenderer sprite;
    private Animator anim;
    private float timer = 0;
    private Vector2? worldCheckVector = null;
    private Vector2 spawn = new Vector2(0.0f,0.0f);
    private HealthBar healthBar;

    private void Awake()
    {
        spawn = transform.position;
        GameObject healthBarObject = Instantiate(healthBarInstance);
        healthBarObject.transform.SetParent(GameObject.Find("Canvas").transform);
        healthBar = healthBarObject.GetComponent<HealthBar>();
        healthBar.SetHealthBar(maxHealth);
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

        if(Input.GetKeyDown(KeyCode.P))
        {
            Attack();
        }
    }

    private void Attack()
    {

        anim.SetTrigger("Attack");
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position,attackRange,enemyLayers);

        foreach(Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyLogic>().TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawSphere(attackPoint.position, attackRange);
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
        healthBar.RestoreHealth();
    }

    private void Hit()
    {
        Debug.Log("Hit");
        if (healthBar.Damage(1)) Death();
        // Update health
        NeedUpdateHealth = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Enemy") && !other.isTrigger)
            Hit();

        if (other.gameObject.tag.Equals("Death"))
            Death();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("PlayerInternal") && this != collision.gameObject)
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
