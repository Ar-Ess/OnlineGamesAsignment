using UnityEngine;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using System.Net;
using System.Threading;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using System.IO;

public class UDPClient : MonoBehaviour
{
    private static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    EndPoint ep;
    bool connectToScene = false;
    bool onLobby = false;
    private string serverIP;
    string stringData = "Hello";
    byte[] data = new byte[1024];
    Thread thr;
    Thread snd;
    Thread rcv;
    public InputField field;
    private PlayerMovement localPlayer = null;
    MemoryStream recvStream = new MemoryStream();
    bool getPlayer = false;
    private Serializer serializer = new Serializer();
    [SerializeField] private GameObject onlinePlayer;
    private List<OnlinePlayer> players = new List<OnlinePlayer>();

    private UDPClient _instance;
    public UDPClient Instance { get { return _instance; } }

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
        if (!onLobby) UpdateJoinServer();
    }

    private void UpdateJoinServer()
    {
        if (true) //TODO: Keep track of amount of players built and do a if here to check if all builts done. so program does not have to iterate foreach every time
        {
            foreach (OnlinePlayer player in players)
            {
                if (!player.built) player.SetOnlinePlayer(Instantiate(onlinePlayer));
            }
        }

        if (getPlayer && localPlayer == null)
        {
            localPlayer = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<PlayerMovement>();
            getPlayer = false;
        }
        if (connectToScene) ChangeScene("LobbyScene");
    }

    public void GetPlayersInfo(ref int numberPlayers, ref int maximumPlayers)
    {
        numberPlayers = players.Count + 1;
        maximumPlayers = 2;
    }

    private void ConnectClient()
    {
        clientSocket.Connect(ep);

        if (!clientSocket.Connected)
        {
            Debug.Log("UDP Server not created.");
            return;
        }

        connectToScene = true;

        data = Encoding.ASCII.GetBytes(stringData);
        clientSocket.Send(data, data.Length, SocketFlags.None);
        rcv.Start();
        snd.Start();
        thr.Abort();
    }

    private void Receive()
    {
        while (true)
        {
            foreach (OnlinePlayer player in players)
            {
                if (!player.built) continue;

                EndPoint ip = player.ep;
                byte[] buffer = new byte[1024];

                clientSocket.ReceiveFrom(buffer, ref ip);
                recvStream = new MemoryStream(buffer);
                //uint recUint = serializer.Deserialize(recvStream);
                //player.movement.SetFlag(recUint);
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

    private void Send()
    {
        while (true)
        {
            if (!localPlayer) continue;

            //if (!localPlayer.IsAnyInputActive()) continue;

            //clientSocket.SendTo(serializer.Serialize(localPlayer.GetFlag()).GetBuffer(), ep);
            //localPlayer.ClearFlag();

            if (localPlayer.IsAnyInputActive())
            {
                clientSocket.SendTo(serializer.SerializeFlag(localPlayer.GetFlag()).GetBuffer(), ep);

                localPlayer.ClearFlag();
            }

            if (localPlayer.IsSendingWorldCheck() && localPlayer.GetWorldCheck() != null)
            {
                clientSocket.SendTo(serializer.SerializeVector(localPlayer.GetWorldCheck().GetValueOrDefault()).GetBuffer(), ep);
                
                localPlayer.ClearWorldCheckVector();
            }
        }

        snd.Abort();
    }

    public void JoinServer()
    {
        ep = new IPEndPoint(IPAddress.Parse(serverIP), 5554);
        thr = new Thread(new ThreadStart(ConnectClient));
        rcv = new Thread(new ThreadStart(Receive));
        snd = new Thread(new ThreadStart(Send));
        thr.Start();
    }

    private void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);

        if (scene == "LobbyScene")
        {
            connectToScene = false;
            getPlayer = true;
            players.Add(new OnlinePlayer((IPEndPoint)ep, true));
            onLobby = true;
        }
    }

    public void ChangeStringIP()
    {
        serverIP = field.text;
        Debug.Log("ip: " + serverIP.ToString());
    }

    private void OnApplicationQuit()
    {
        if (thr != null) thr.Abort();
        if (rcv != null) rcv.Abort();
        if (snd != null) snd.Abort();
        if (clientSocket.Connected) clientSocket.Disconnect(false);
        clientSocket.Close();
    }
}
