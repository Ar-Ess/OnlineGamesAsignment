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
        UdpClient client = new UdpClient();
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse("192.168.2.25"), 5554);
        client.Connect(ep);

        client.Send(new byte[] { 1, 2, 3, 4, 5 }, 5);
        var receivedData = client.Receive(ref ep);
        Debug.Log("received data from" + ep.ToString());

        connected = true; ;
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }

    
}
