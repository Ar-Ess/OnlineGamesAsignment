using UnityEngine;
using System.Net;

class OnlinePlayer
{
    public int health { get { return movement.onlineHealth.health; } set { movement.onlineHealth.UpdateOnlineHealth(value); } }

    public OnlinePlayer(IPEndPoint ep, bool server = false)
    {
        this.ep = ep;
        this.player = null;
        this.movement = null;
        this.built = new bool();
        this.built = false;
        this.informed = new bool();
        this.informed = false;
        this.server = new bool();
        this.server = server;
    }

    public void BuildOnlinePlayer(GameObject player)
    {
        this.player = player;
        this.movement = player.GetComponentInChildren<OnlinePlayerMovement>();
        this.built = true;
    }

    public bool IsPlayerNotInformed()
    {
        return (built && !informed);
    }

    public bool built;
    public bool informed;
    public bool server;
    public IPEndPoint ep;
    public GameObject player;
    public OnlinePlayerMovement movement;
};
