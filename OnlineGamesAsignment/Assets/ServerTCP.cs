using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class ServerTCP : MonoBehaviour
{
    Socket newSocket;
    Socket client;
    IPEndPoint ipep;
    int port;
    byte[] data;

    private void Awake()
    {
        port = 5556;
        data = new byte[1024];
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ipep = new IPEndPoint(IPAddress.Any, port);
    }
    // Start is called before the first frame update
    void Start()
    {
        newSocket.Bind(ipep);
        newSocket.Listen(10);
        client = newSocket.Accept();
        newSocket.Connect(ipep);
    }

    // Update is called once per frame
    void Update()
    {
        int recv = client.Receive(data);
        client.Send(data, recv, SocketFlags.None);
    }
}
