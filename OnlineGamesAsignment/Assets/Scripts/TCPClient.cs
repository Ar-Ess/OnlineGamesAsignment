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

public class TCPClient : MonoBehaviour
{
    private static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    bool connected = false;
    bool connecting = false;
    int attempts = 0; 

    private void Update()
    {
        
        if (connecting)
        {
            Debug.Log("trying to connect");
            LoopConnect();
            Debug.Log("connected");
        }      
        
    }

    public void LoopConnect()
    {
        attempts++;

        try
        {
            clientSocket.Connect(IPAddress.Loopback, 5555);
            connecting = false;
            Debug.Log("Connected!");
            attempts = 0;
            ChangeScene();
        }
        catch (SocketException)
        {
                Debug.LogError("Connection attempts: " + attempts.ToString());
        }


    }

    public void StartConnection()
    {
        connecting = true;
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
}
