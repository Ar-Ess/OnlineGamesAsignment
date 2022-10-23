using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class UDPServer : MonoBehaviour
{
    int recv;
    byte[] data = new byte[1024];
    IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 5554);
    Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    EndPoint remote;
    Thread thr;
    [SerializeField] int maxPlayers;
    public void SetupServer()
    {

        Debug.Log("Setting up server...");
        SceneManager.LoadScene(1);
        serverSocket.Bind(ipep);
        Debug.Log("Waiting for client...");

        thr = new Thread(new ThreadStart(Listen));
        thr.Start();

    }
    private void Listen()
    {
        int numPlayers = 1;

        while(numPlayers < maxPlayers)
        {
            IPEndPoint clientSocket = new IPEndPoint(IPAddress.Any, 0);
            remote = clientSocket;

            recv = serverSocket.ReceiveFrom(data, ref remote);

            numPlayers++;

            Debug.Log("Message received from {0}: " + remote.ToString());
            Debug.Log("Welcome to NoNameServer!");

            string welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            serverSocket.SendTo(data, data.Length, SocketFlags.None, remote);
        }
        thr.Abort();
    }

    
}
