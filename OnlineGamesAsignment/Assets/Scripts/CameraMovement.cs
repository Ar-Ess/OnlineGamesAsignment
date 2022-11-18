using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
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
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y,-9.8f);
    }
}
