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
using System.Threading;

public class TCPClient : MonoBehaviour
{
    private static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    bool connectToScene = false;
    int attempts = 0;
    Thread thr;

    private TCPClient _instance;
    public TCPClient Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }

        else _instance = this;

        DontDestroyOnLoad(this);

    }

    private void Update()
    {
        if (connectToScene) ChangeScene();
    }

    public void ConnectClient()
    {
        attempts++;

        try
        {
            clientSocket.Connect(IPAddress.Loopback, 5555);
            if (clientSocket.Connected)
            {
                attempts = 0;
                connectToScene = true;
                thr.Abort();
            }
            else
                Debug.Log("TCP Server not created.");
        }
        catch (SocketException)
        {
                Debug.LogError("Connection attempts: " + attempts.ToString());
        }


    }

    public void JoinServer()
    {
        thr = new Thread(new ThreadStart(ConnectClient));
        thr.Start();
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(1);
        connectToScene = false;
    }

    private void OnApplicationQuit()
    {
        if (thr != null) thr.Abort();
        if (clientSocket.Connected) clientSocket.Disconnect(false);
        clientSocket.Close();
    }
}
