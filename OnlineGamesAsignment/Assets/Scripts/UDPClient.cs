using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class UDPClient : MonoBehaviour
{
    private static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
    bool connected = false;

    private void Start()
    {
        SendDeita();
    }

    void Update()
    {
        if (connected) ChangeScene();
       
    }

    private void SendDeita()
    {
        byte[] buffer = new byte[1024];
        clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Sen), clientSocket);
    }
    private void Sen(IAsyncResult AR)
    {
        clientSocket.EndSend(AR);
        byte[] buffer = new byte[1024];
        clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Resif), clientSocket);
    }

    private void Resif(IAsyncResult AR)
    {
        clientSocket.EndReceive(AR);
        byte[] buffer = new byte[1024];
        clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Sen), clientSocket);
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
}
