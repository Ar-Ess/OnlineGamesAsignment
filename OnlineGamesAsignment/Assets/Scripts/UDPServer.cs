using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UDPServer : MonoBehaviour
{
    byte[] data = new byte[1024];
    IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 5554);
    Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    EndPoint remote;
    Thread thr;
    Thread snd;
    [SerializeField] int maxPlayers;
    string stringData = string.Empty;

    ServerState status;

    private List<IPEndPoint> clientsUDP = new List<IPEndPoint>();

    private UDPServer _instance;
    public UDPServer Instance { get { return _instance; } }

    enum ServerState
    {
        LISTENING,
        SENDING,
        RECIEVING,

    }

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
        if(status == ServerState.SENDING)
        {
            snd.Start();
        }
    }

    public void SetupServer()
    {
        Debug.Log("Setting up udp server...");
        ChangeScene();
        serverSocket.Bind(ipep);

        Debug.Log("Waiting for client...");
        thr = new Thread(new ThreadStart(Listen));
        snd = new Thread(new ThreadStart(Send));
        thr.Start();

        status = ServerState.LISTENING;
    }
    private void Listen()
    {
        int numPlayers = 1;

        while(numPlayers < maxPlayers)
        {
            IPEndPoint clientSocket = new IPEndPoint(IPAddress.Any, 0);
            Debug.Log(clientSocket.Address);
            remote = clientSocket;
            serverSocket.ReceiveFrom(data, ref remote);
            stringData = Encoding.ASCII.GetString(data);

            if (data != null)
            {
                numPlayers++;
                clientsUDP.Add((IPEndPoint)remote);
            }

            numPlayers++;

            Debug.Log("Message received from {0}: " + remote.ToString());
            Debug.Log("Message: " + stringData);
        }
        status = ServerState.SENDING;
        thr.Abort();
    }

    private void Send()
    {
        stringData = "Welcome to NoNameServer";
        data = Encoding.ASCII.GetBytes(stringData);
        foreach(IPEndPoint i in clientsUDP)
        {
            serverSocket.SendTo(data, i);
        }
        status = ServerState.RECIEVING;
        snd.Abort();
    }

    private void OnApplicationQuit()
    {
        if (thr != null) thr.Abort();
        if (snd != null) snd.Abort();
        if (serverSocket.Connected) serverSocket.Disconnect(false);
        serverSocket.Close();
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }


}
