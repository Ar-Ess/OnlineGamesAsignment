using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Vector3 m_CameraOffset = new Vector3(0, 0, 0);
    private Transform player = null;

    // Update is called once per frame
    void Update()
    {
        if (player == null) 
        {
            GameObject obj = GameObject.FindGameObjectWithTag("LocalPlayer");
            if (obj != null) player = obj.transform.Find("Physics");
        }
        else Movement();
    }

    private void Movement()
    {
        transform.position = new Vector3(player.transform.position.x + m_CameraOffset.x, player.transform.position.y + m_CameraOffset.y, m_CameraOffset.z);
    }
}
