using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagerSceneLogic : MonoBehaviour
{
    [SerializeField] private GameObject localPlayer = null;

    void Start()
    {
        Instantiate(localPlayer);
        GameObject.FindGameObjectWithTag("OnlinePLayer").GetComponentInChildren<OnlinePlayerMovement>().InstantiateHealthBar();
    }
}
