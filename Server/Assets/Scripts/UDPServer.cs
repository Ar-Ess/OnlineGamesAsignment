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
    private static List<Socket> clientSockets = new List<Socket>();
    private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
    private static byte[] buffer = new byte[1024];
    EndPoint eP;

    void Start()
    {
        SetapServer();
    }

    private void SetapServer()
    {
        Debug.Log("Setting up server...");
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 5555);
        eP = endPoint;

        serverSocket.Bind(endPoint);
        serverSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref eP, new AsyncCallback(ProcesDeitagram), null);
    }

    private void ProcesDeitagram(IAsyncResult AR)
    {
        Debug.Log("message resifd");
        int receivedData = serverSocket.EndReceive(AR);
        byte[] tempBuff = new byte[receivedData];
        Array.Copy(buffer, tempBuff, receivedData);

        string text = Encoding.ASCII.GetString(buffer);
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

        serverSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SenColbac), null);
        serverSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref eP, new AsyncCallback(ProcesDeitagram), AR.AsyncState);
    }


    private void SenColbac(IAsyncResult AR)
    {
        Socket socket = (Socket)AR.AsyncState;
        socket.EndSend(AR);
    }
}
