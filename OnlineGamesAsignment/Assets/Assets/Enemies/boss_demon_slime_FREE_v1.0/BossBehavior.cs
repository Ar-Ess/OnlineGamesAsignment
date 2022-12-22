using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    // Adjustable variables for customization
    public float moveSpeed = 2f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public float attackDuration = 0.5f;
    public float attackDamage = 10f;
    public float health = 100f;
    public GameObject attackPrefab;
    public Transform attackSpawnPoint;

    // Private variables
    private float lastAttackTime;
    private Rigidbody2D rb;
    private Animator anim;
    private GameObject player;
    private bool isAttacking;
    private bool isDead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
    }

    void Update()
    {
        // Ideally this goes in the start method, however because the localplayer clone gets instantiated after, needs a better solution than this.
        player = GameObject.FindWithTag("LocalPlayer");

        // Check if the boss is dead
        if (health <= 0 && !isDead)
        {
            isDead = true;
            anim.SetTrigger("Die");
            StartCoroutine(Death());
            
        }

        // Only perform actions if the boss is not attacking and is not dead
        if (!isAttacking && !isDead)
        {
            // Check if the player is within attack range
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= attackRange)
            {
                // Attack the player
                StartCoroutine(Attack());
            }
            else
            {
                // Move towards the player
                Vector2 direction = player.transform.position - transform.position;
                rb.velocity = direction.normalized * moveSpeed;

                // Update the animator's movement direction
                anim.SetFloat("Horizontal", direction.x);
                anim.SetFloat("Vertical", direction.y);
                anim.SetBool("Moving", true);
            }
        }
        else
        {
            // Stop moving
            rb.velocity = Vector2.zero;
            anim.SetBool("Moving", false);
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;

        // Check if the attack hit the player
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= attackRange)
        {
            // Player takes damage
        }
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(2.2f);
        Destroy(gameObject, 1f);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        anim.SetTrigger("Hit");
    }
}