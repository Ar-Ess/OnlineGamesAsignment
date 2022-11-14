using UnityEngine;
using System.Net;

class OnlinePlayer
{
    public OnlinePlayer(IPEndPoint ep)
    {
        this.ep = ep;
        player = null;
        movement = null;
        built = new bool();
        built = false;
    }

    public OnlinePlayer(IPEndPoint ep, GameObject player)
    {
        this.ep = ep;
        this.player = player;
        movement = player.GetComponent<OnlinePlayerMovement>();
        built = new bool();
        built = true;
    }

    public void SetOnlinePlayer(GameObject player)
    {
        this.player = player;
        this.movement = player.GetComponent<OnlinePlayerMovement>();
        built = true;
    }

    public bool built;
    public IPEndPoint ep;
    public GameObject player;
    public OnlinePlayerMovement movement;
};
