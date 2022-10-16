using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System;

public class TCPServer : MonoBehaviour
{
    private static List<Socket> clientSockets = new List<Socket>();
    private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private static byte[] buffer = new byte[1024];
    // Start is called before the first frame update
    void Start()
    {
        SetupServer();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetupServer()
    {
        Debug.Log("Setting up server...");
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5555));
        serverSocket.Listen(1);
        serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
    }

    private void AcceptCallback(IAsyncResult AR)
    {
        Socket socket = serverSocket.EndAccept(AR);
        clientSockets.Add(socket);
        Debug.Log("Client Connected!");
        socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
    }

    private static void ReceiveCallback(IAsyncResult AR)
    {
        Socket socket = (Socket)AR.AsyncState;
        int receivedData = socket.EndReceive(AR);
        byte[] tempBuff = new byte[receivedData];
        Array.Copy(buffer, tempBuff, receivedData);

        string text = Encoding.ASCII.GetString(tempBuff);
        string response = string.Empty;
        Debug.Log("Text Received: " + text);

        if (text.ToLower() != "get time")
        {
            response = "Invalid request";
        }
        else
        {
            response = DateTime.Now.ToLongTimeString();
        }

        byte[] data = Encoding.ASCII.GetBytes(response);
        socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
        socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
    }


    private static void SendCallback(IAsyncResult AR)
    {
        Socket socket = (Socket)AR.AsyncState;
        socket.EndSend(AR);
    }
}
