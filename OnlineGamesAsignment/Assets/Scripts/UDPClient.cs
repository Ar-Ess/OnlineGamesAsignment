using UnityEngine;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using System.Net;
using System.Collections;
using System.Threading;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using System.IO;

public class UDPClient : MonoBehaviour
{
    [SerializeField] private GameObject onlinePlayer = null;
    public UDPClient Instance { get { return _instance; } }
    public uint MaxLobbyPlayers { get { return maxLobbyPlayers; } }
    public uint NumLobbyPlayers { get { return (uint)players.Count + 1; } }

    // Private
    private Socket clientSocket;
    private Thread cnct, snd, rcv;
    private uint maxLobbyPlayers = 1;
    private PlayerMovement localPlayer = null;
    private List<OnlinePlayer> players = new List<OnlinePlayer>();
    private UDPClient _instance;
    private EndPoint serverEndPoint = null;
    // BuildPlayer (0) | JoinServer (1) | CreateNewPlayer (2) |
    private StreamFlag callbacks = new StreamFlag(0); 

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else _instance = this;
        DontDestroyOnLoad(this);

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        cnct = new Thread(new ThreadStart(ConnectClient));
        rcv = new Thread(new ThreadStart(Receive));
        snd = new Thread(new ThreadStart(Send));
    }

    private void Update()
    {
        AddPlayerLogic(); // Ha d'estar justament aqu�. A sobre de tot. No moure.

        JoinServerLogic();
        LookForLocalPlayerInstance(); //TODO: Only in lobby
        BuildPlayers();
    }

    private void AddPlayerLogic()
    {
        if (!callbacks.Get(2)) return;
        callbacks.Set(2, false);

        players.Add(new OnlinePlayer((IPEndPoint)serverEndPoint, players.Count == 0));
        callbacks.Set(0, true);
    }

    private void JoinServerLogic()
    {
        if (!callbacks.Get(1)) return;
        callbacks.Set(1, false);

        ChangeScene("LobbyScene");
        AddNewPlayer();
    }

    private void LookForLocalPlayerInstance()
    {
        if (localPlayer != null) return;

        GameObject obj = GameObject.FindGameObjectWithTag("LocalPlayer");
        if (obj != null) localPlayer = obj.GetComponent<PlayerMovement>();
    }

    private void BuildPlayers()
    {
        if (!callbacks.Get(0)) return;
        callbacks.Set(0, false);

        foreach (OnlinePlayer player in players)
        {
            if (!player.built)
                player.BuildOnlinePlayer(Instantiate(onlinePlayer));
        }
    }

    private void ConnectClient()
    {
        clientSocket.Connect(serverEndPoint);

        if (!clientSocket.Connected) return;

        // JoinServer
        callbacks.Set(1, true);

        byte[] data = Encoding.ASCII.GetBytes("Client");
        clientSocket.Send(data, data.Length, SocketFlags.None);
        rcv.Start();
        snd.Start();
    }

    private void Receive()
    {
        while (true)
        {
            byte[] buffer = new byte[1024];
            clientSocket.ReceiveFrom(buffer, ref serverEndPoint);
            MemoryStream recvStream = new MemoryStream(buffer);

            foreach (OnlinePlayer player in players)
            {
                if (!player.built) continue;
                DataType type = Serializer.CheckDataType(recvStream);
                if (type != DataType.INPUT_FLAG) Debug.Log(type.ToString());
                switch (type)
                {
                    case DataType.INPUT_FLAG:
                        player.movement.SetFlag(Serializer.Deserialize(recvStream).Uint());
                        break;
                    case DataType.WORLD_CHECK:
                        player.movement.SetPosition(Serializer.Deserialize(recvStream).Vector2());
                        break;
                    case DataType.LOBBY_MAX:
                        maxLobbyPlayers = Serializer.Deserialize(recvStream).Uint();
                        break;
                }
            }

            recvStream.Flush();
            recvStream.Dispose();
        }
    }

    private void Send()
    {
        while (true)
        {
            if (!localPlayer) continue;

            if (localPlayer.IsAnyInputActive())
            {
                SendData(Serializer.Serialize(localPlayer.GetFlag(), DataType.INPUT_FLAG));
                localPlayer.ClearFlag();
            }

            if (localPlayer.IsSendingWorldCheck() && localPlayer.GetWorldCheck() != null)
            {
                SendData(Serializer.Serialize(localPlayer.GetWorldCheck().GetValueOrDefault()));
                localPlayer.ClearWorldCheckVector();
            }
        }
    }

    public void JoinServer(InputField field)
    {
        IPAddress adress;
        if (!IPAddress.TryParse(field.text, out adress)) return;

        serverEndPoint = new IPEndPoint(adress, 5554);
        cnct.Start();
    }

    private void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    private void AddNewPlayer()
    {
        callbacks.Set(2, true);
    }

    private void OnApplicationQuit()
    {
        if (cnct != null) cnct.Abort();
        if (rcv != null) rcv.Abort();
        if (snd != null) snd.Abort();
        if (clientSocket.Connected) clientSocket.Disconnect(false);
        clientSocket.Close();
    }

    private void SendData(byte[] data)
    {
        clientSocket.SendTo(data, serverEndPoint);
    }
}
