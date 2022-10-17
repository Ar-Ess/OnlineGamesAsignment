using UnityEngine;
using System.Net.Sockets;
using System;
using UnityEngine.SceneManagement;
using System.Net;

public class UDPClient : MonoBehaviour
{
    private static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    bool connected = false;
    IPEndPoint endPoint;

    private void Start()
    {
        SendData();
    }

    void Update()
    {
        if (connected) ChangeScene();
       
    }
    public void SendData()
    {
        byte[] buffer = new byte[1024];
        endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.1"), 5554);
        Debug.Log(endPoint.ToString());
        clientSocket.BeginSendTo(buffer, 0, buffer.Length, SocketFlags.None, endPoint, new AsyncCallback(Send), clientSocket);
    }

    private void Send(IAsyncResult AR)
    {
        clientSocket.EndSendTo(AR);
        byte[] buffer = new byte[1024];
        EndPoint eP = endPoint;
        clientSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref eP, new AsyncCallback(Recieve), clientSocket);
    }

    private void Recieve(IAsyncResult AR)
    {
        clientSocket.EndReceive(AR);
        connected = true;
        SendData();
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
}
