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
    private static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    bool connected = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void LoopConnect()
    {
        int attempts = 0;
        while(!clientSocket.Connected)
        {
            try
            {
                attempts++;
                clientSocket.Connect(IPAddress.Loopback, 5555);
                connected = true;
            }
            catch (SocketException)
            {
                Debug.Log("Connection attempts: " + attempts.ToString());
            }
        }

        Debug.ClearDeveloperConsole();
        Debug.Log("Connected!");

    }

    // Update is called once per frame
    void Update()
    {
        if (connected) ChangeScene();
       
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
}
