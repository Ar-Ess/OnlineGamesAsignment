using UnityEngine;
using System.Net;

class OnlinePlayer
{
    public OnlinePlayer(IPEndPoint ep, bool server = false)
    {
        this.ep = ep;
        this.player = null;
        this.movement = null;
        this.built = new bool();
        this.built = false;
        this.server = new bool();
        this.server = server;
    }

    public void SetOnlinePlayer(GameObject player)
    {
        this.player = player;
        this.movement = player.GetComponent<OnlinePlayerMovement>();
        this.built = true;
    }

    public bool built;
    public bool server;
    public IPEndPoint ep;
    public GameObject player;
    public OnlinePlayerMovement movement;
};
