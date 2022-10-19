using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System;

public class UDPServer : MonoBehaviour
{
    int recv;
    byte[] data = new byte[1024];
    IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 5554);
    Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

    private void Start()
    {

    }

    public void SetupServer()
    {
        Debug.Log("Setting up server...");
        serverSocket.Bind(ipep);
        Debug.Log("Waiting for client...");

        IPEndPoint clientSocket = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)(clientSocket);
        recv = serverSocket.ReceiveFrom(data, ref remote);
        Debug.Log("Message received from {0}: " + remote.ToString());
        Debug.Log(Encoding.ASCII.GetString(data,0,recv));

        string welcome = "Welcome to my test server";
        data = Encoding.ASCII.GetBytes(welcome);
        serverSocket.SendTo(data, data.Length, SocketFlags.None, remote);
        while(true)
        {
            data = new byte[1024];
            recv = serverSocket.ReceiveFrom(data, ref remote);

            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
            serverSocket.SendTo(data, recv, SocketFlags.None, remote);
        }

    }
}
