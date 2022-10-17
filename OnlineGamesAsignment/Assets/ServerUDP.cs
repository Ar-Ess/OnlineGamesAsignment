using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class ServerUDP : MonoBehaviour
{
    Socket newSocket;

    int port;
    IPEndPoint ipep;
    byte[] data;
    IPEndPoint remote;
    int recv;
  
    private void Awake()
    {
        newSocket = new Socket(AddressFamily.InterNetwork,
                            SocketType.Dgram, ProtocolType.Udp);
        port = 5555;

        ipep = new IPEndPoint(IPAddress.Any, port);

        data = new byte[1024];

        remote = new IPEndPoint(IPAddress.Any, port);

    }

    // Start is called before the first frame update
    void Start()
    {
        newSocket.Bind(ipep);
    }

    // Update is called once per frame
    void Update()
    {
        EndPoint remote = new IPEndPoint(IPAddress.Any, port);
        byte[] data = new byte[1024];
        int recv = newSocket.ReceiveFrom(data, ref remote);
        newSocket.SendTo(data, recv, SocketFlags.None, remote);


        newSocket.SendTo(data, recv, SocketFlags.None, remote);
    }
}
