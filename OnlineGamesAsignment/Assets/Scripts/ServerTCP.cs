using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class ServerTCP : MonoBehaviour
{
    Socket newSocket;
    Socket client;
    IPEndPoint ipep;
    int port;
    byte[] data;
    Thread listenThread;

    private void Awake()
    {
        port = 5556;
        data = new byte[1024];
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ipep = new IPEndPoint(IPAddress.Any, port);
        listenThread = new Thread(Listen);
    }
    // Start is called before the first frame update
    void Start()
    {
        newSocket.Bind(ipep);
        client = newSocket.Accept();
        newSocket.Connect(ipep);
        listenThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        listenThread.Start();
        int recv = client.Receive(data);
        client.Send(data, recv, SocketFlags.None);
    }

    void Listen()
    {
        newSocket.Listen(10);
    }
}
