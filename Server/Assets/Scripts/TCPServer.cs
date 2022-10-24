using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using UnityEngine.SceneManagement;

public class TCPServer : MonoBehaviour
{
    private static List<Socket> clientSockets = new List<Socket>();
    private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private static byte[] buffer = new byte[1024];
    private TCPServer _instance;
    public TCPServer Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }

        else _instance = this;

        DontDestroyOnLoad(this);
        
    }


    public void SetupServer()
    {
        Debug.Log("Setting up tcp server...");
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5555));
        ChangeScene();

        Debug.Log("Waiting for client...");
        serverSocket.Listen(2);
        serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

    }

    private void AcceptCallback(IAsyncResult AR)
    {
        Socket socket = serverSocket.EndAccept(AR);
        clientSockets.Add(socket);
        Debug.Log("Client Connected!");
        socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        Debug.Log(" Welcome to NoNameServer!");
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

    private void OnApplicationQuit()
    {
        if (serverSocket.Connected) serverSocket.Disconnect(false);
        serverSocket.Close();
        foreach(Socket socket in clientSockets)
        {
            if (socket.Connected) socket.Disconnect(false);
            socket.Close();
        }
        clientSockets.Clear();
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
}
