using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Vector3 m_CameraOffset = new Vector3(0, 0, 0);
    private GameObject player = null;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("LocalPlayer");

        Movement();
    }

    private void Movement()
    {
        transform.position = new Vector3(player.transform.position.x + m_CameraOffset.x, player.transform.position.y + m_CameraOffset.y, m_CameraOffset.z);
    }
}
