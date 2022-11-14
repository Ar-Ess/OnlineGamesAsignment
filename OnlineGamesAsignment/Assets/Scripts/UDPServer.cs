using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class UDPServer : MonoBehaviour
{
    byte[] data = new byte[1024];
    IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 5554);
    Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    EndPoint remote;
    Thread thr;
    Thread snd;
    Thread rcv;
    [SerializeField] int maxPlayers;
    string stringData = string.Empty;
    PlayerMovement localPlayer = null;
    MemoryStream recvStream = new MemoryStream();
    private Serializer serializer = new Serializer();
    [SerializeField] private GameObject onlinePlayer;
    private List<OnlinePlayer> clientsUDP = new List<OnlinePlayer>();

    private UDPServer _instance;
    public UDPServer Instance { get { return _instance; } }

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
        if (true) //TODO: Keep track of amount of players built and do a if here to check if all builts done. so program does not have to iterate foreach every time
        {
            foreach (OnlinePlayer player in clientsUDP)
            {
                if (!player.built) player.SetOnlinePlayer(Instantiate(onlinePlayer));
            }
        }
        if (localPlayer == null)
        {
            localPlayer = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<PlayerMovement>();
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
        rcv = new Thread(new ThreadStart(Receive));
        thr.Start();
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
                clientsUDP.Add(new OnlinePlayer((IPEndPoint)remote));
            }

            Debug.Log("Message received from {0}: " + remote.ToString());
            Debug.Log("Message: " + stringData);
            rcv.Start();
            snd.Start();
        }
         
        thr.Abort();
    }

    private void Send()
    {
        while (true)
        {
            if (!localPlayer) continue;
            if (!localPlayer.IsAnyInputActive()) continue;

            foreach (OnlinePlayer player in clientsUDP)
            {
                if (!player.built) continue;
                serverSocket.SendTo(serializer.Serialize(localPlayer.GetFlag()).GetBuffer(), player.ep);
            }
        }

        snd.Abort();
    }

    private void Receive()
    {
        while(true)
        {
            foreach(OnlinePlayer player in clientsUDP)
            {
                if (!player.built) continue;
                EndPoint ip = player.ep;
                byte[] buffer = new byte[1024];

                serverSocket.ReceiveFrom(buffer, ref ip);
                recvStream = new MemoryStream(buffer);
                uint recUint = serializer.Deserialize(recvStream);
                //Debug.Log("Received message from: " + remote.ToString());
                //Debug.Log("Message: " + recUint);
                player.movement.SetFlag(recUint);
            }
            
        }
        
    }

    private void OnApplicationQuit()
    {
        if (thr != null) thr.Abort();
        if (snd != null) snd.Abort();
        if (rcv != null) rcv.Abort();
        if (serverSocket.Connected) serverSocket.Disconnect(false);
        serverSocket.Close();
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }


}
