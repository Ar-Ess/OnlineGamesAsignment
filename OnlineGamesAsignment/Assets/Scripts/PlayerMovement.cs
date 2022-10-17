using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] Camera camera;
    [SerializeField] Transform spawnpoint;

    [Header("Physics")]
    [SerializeField] float speed = 6000;
    [SerializeField] float acceleration = 1;
    [SerializeField] float jumpForce = 5000;
    [SerializeField] float globalGravity = 10;

    float gravityScale = 10;
    bool ground = false;

    Rigidbody rb;

    void Start()
    {
        camera.transform.position = new Vector3(0, 33, -36);
        camera.transform.rotation = Quaternion.Euler(13, 0, 0);
        transform.position = spawnpoint.position;
        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -200, 0);
        rb.useGravity = false;
        ground = false;
    }

    private void FixedUpdate()
    {
        camera.transform.position = new Vector3(transform.position.x, transform.position.y + 33, -36);

        Vector3 gravity = -globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    void Update()
    {

        float acc = 1;
        if (Input.GetKey(KeyCode.LeftShift)) acc = acceleration;

        if (Input.GetKey(KeyCode.A)) rb.AddForce(new Vector3(-speed * Time.deltaTime * acc, 0, 0));

        if (Input.GetKey(KeyCode.D)) rb.AddForce(new Vector3(speed * Time.deltaTime * acc, 0, 0));

        if (Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.CompareTag("Ground"))
        {
            ground = true;
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode.Acceleration);
        ground = false;
    }
}
