using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySceneLogic : MonoBehaviour
{
    [SerializeField] private GameObject localPlayer;
    void Start()
    {
        Instantiate(localPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
