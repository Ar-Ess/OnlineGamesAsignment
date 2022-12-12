using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    private enum Behaviour
    {
        STILL,
        MOVE_FORWARD,
        OSCILLATE
    }

    [Header("General")]
    [SerializeField] private Behaviour behaviour = Behaviour.STILL;
    [SerializeField] private Vector2 velocity = new Vector2(0, 0);

    [Header("Move Forward")]
    [SerializeField] private bool startOnTriggerEnter = false;
    [SerializeField] private string triggerTag = "";

    [Header("Oscillate Config")]
    [SerializeField] private Vector2 amplitude = new Vector2(0.0f, 0.0f);

    // Private
    private SpriteRenderer sprite;
    private Vector2 topRightLimit = new Vector2(0, 0);
    private Vector2 bottomLeftLimit = new Vector2(0, 0);
    private bool moveForward = false;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        topRightLimit = new Vector2(transform.position.x + amplitude.x, transform.position.y + amplitude.y);
        bottomLeftLimit = new Vector2(transform.position.x - amplitude.x, transform.position.y - amplitude.y);
        sprite.flipX = (velocity.x < 0);
        moveForward = !startOnTriggerEnter;
    }

    // Update is called once per frame
    void Update()
    {
        switch (behaviour)
        {
            case Behaviour.STILL:
                break;
            case Behaviour.MOVE_FORWARD:
                MoveForward();
                break;
            case Behaviour.OSCILLATE:
                OscillateLogic();
                break;
        }
    }

    private void MoveForward()
    {
        if (!moveForward) return;

        transform.position += new Vector3(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, 0);
    }

    private void OscillateLogic()
    {
        Vector3 vel = new Vector3(0, 0, 0);

        vel.x += velocity.x * Time.deltaTime;
        vel.y += velocity.y * Time.deltaTime;

        if (velocity.x != 0) OscillateX();

        if (velocity.y != 0) OscillateY();

        transform.position += vel;
    }

    private void OscillateX()
    {
        if (!((velocity.x > 0 && transform.position.x > topRightLimit.x) || (velocity.x < 0 && transform.position.x < bottomLeftLimit.x))) return;

        sprite.flipX = (velocity.x >= 0);
        velocity.x *= -1;
    }
    private void OscillateY()
    {
        if (!((velocity.y > 0 && transform.position.y > topRightLimit.y) || (velocity.y < 0 && transform.position.y < bottomLeftLimit.y))) return;

        velocity.y *= -1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Death"))
            Destroy(this);

        if (other.tag.Equals("Ground")) return;

        if (behaviour == Behaviour.MOVE_FORWARD && startOnTriggerEnter && other.tag.Equals(triggerTag))
        {
            moveForward = true;
        }
    }
}
