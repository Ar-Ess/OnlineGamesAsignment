using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlacer : MonoBehaviour
{
    [Header("Placement")]
    [SerializeField] private Transform spawnpoint = null;
    [SerializeField] private float globalScale = 1.0f;

    void Awake()
    {
        transform.position = spawnpoint.position;
        transform.localScale = new Vector3(globalScale, globalScale, 1);
    }

    private void Start()
    {
        
    }
}
