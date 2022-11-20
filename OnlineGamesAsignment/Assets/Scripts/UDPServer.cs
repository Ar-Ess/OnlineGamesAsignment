using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class UDPServer : MonoBehaviour
{
    [SerializeField] private Dropdown dropdown;
    [SerializeField] private GameObject onlinePlayer;

    // Private
    private byte[] data = new byte[1024];
    private IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 5554);
    private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private EndPoint remote;
    private Thread thr;
    private Thread snd;
    private Thread rcv;
    private int maxPlayers = 2;
    private string stringData = string.Empty;
    private PlayerMovement localPlayer = null;
    private MemoryStream recvStream = new MemoryStream();
    private Serializer serializer = new Serializer();
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

        LookForLocalPlayerInstance();
    }

    public void GetPlayersInfo(ref int numberPlayers, ref int maximumPlayers)
    {
        numberPlayers = clientsUDP.Count + 1;
        maximumPlayers = maxPlayers;
    }

    private void LookForLocalPlayerInstance()
    {
        if (localPlayer == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("LocalPlayer");
            if (obj != null) localPlayer = obj.GetComponent<PlayerMovement>();
        }
    }

    public void SetNumPlayers()
    {
        maxPlayers = dropdown.value + 2;
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
            //if (!localPlayer.IsAnyInputActive()) continue;

            //foreach (OnlinePlayer player in clientsUDP)
            //{
            //    if (!player.built) continue;
            //    serverSocket.SendTo(serializer.Serialize(localPlayer.GetFlag()).GetBuffer(), player.ep);

            //}

            //localPlayer.ClearFlag();

            if (localPlayer.IsAnyInputActive())
            {
                foreach (OnlinePlayer player in clientsUDP)
                {
                    if (!player.built) continue;
                    serverSocket.SendTo(serializer.SerializeFlag(localPlayer.GetFlag()).GetBuffer(), player.ep);

                }

                localPlayer.ClearFlag();
            }

            if (localPlayer.IsSendingWorldCheck())
            {
                foreach (OnlinePlayer player in clientsUDP)
                {
                    if (!player.built) continue;
                    if (localPlayer.GetWorldCheck() == null) continue;
                    serverSocket.SendTo(serializer.SerializeVector(localPlayer.GetWorldCheck().GetValueOrDefault()).GetBuffer(), player.ep);

                }

                localPlayer.ClearWorldCheckVector();
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
                //uint recUint = serializer.Deserialize(recvStream);
                //player.movement.SetFlag(recUint);
                //Debug.Log("Received message from: " + remote.ToString());
                switch (serializer.CheckDataType(recvStream))
                {
                    case DataType.INPUT_FLAG:
                        player.movement.SetFlag(serializer.DeserializeFlag(recvStream));
                        break;
                    case DataType.WORLD_CHECK:
                        player.movement.SetPosition(serializer.DeserializeVector(recvStream));
                        break;
                }
                recvStream.Flush();
                recvStream.Dispose();
                
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
