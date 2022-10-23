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
    [SerializeField] private string serverIP;
    void Update()
    {
       
    }
    public void SendData()
    {
        UdpClient client = new UdpClient();
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(serverIP), 5554);
        client.Connect(ep);

        client.Send(new byte[] { 1, 2, 3, 4, 5 }, 5);
        var receivedData = client.Receive(ref ep);
        Debug.Log("received data from" + ep.ToString());

        ChangeScene();
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }

    
}
