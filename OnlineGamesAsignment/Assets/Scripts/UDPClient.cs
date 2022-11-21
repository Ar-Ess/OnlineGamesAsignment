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
    [SerializeField] private GameObject onlinePlayer;
    public UDPClient Instance { get { return _instance; } }
    public uint MaxLobbyPlayers { get { return maxLobbyPlayers; } }
    public uint NumLobbyPlayers { get { return (uint)players.Count + 1; } }

    // Private
    private Socket clientSocket;
    private Thread thr, snd, rcv;
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
        thr = new Thread(new ThreadStart(ConnectClient));
        rcv = new Thread(new ThreadStart(Receive));
        snd = new Thread(new ThreadStart(Send));
    }

    private void Update()
    {
        AddPlayerLogic(); // Ha d'estar justament aquí. A sobre de tot. No moure.

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
                player.SetOnlinePlayer(Instantiate(onlinePlayer));
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
            int size = players.Count;
            for (int i = 0; i < size; ++i)
            {
                OnlinePlayer player = players[i];
                if (!player.built) continue;

                byte[] buffer = new byte[1024];
                clientSocket.ReceiveFrom(buffer, ref serverEndPoint);
                MemoryStream recvStream = new MemoryStream(buffer);

                switch (Serializer.CheckDataType(recvStream))
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

            if (localPlayer.IsAnyInputActive())
            {
                clientSocket.SendTo(Serializer.Serialize(localPlayer.GetFlag(), DataType.INPUT_FLAG), serverEndPoint);
                localPlayer.ClearFlag();
            }

            if (localPlayer.IsSendingWorldCheck() && localPlayer.GetWorldCheck() != null)
            {
                clientSocket.SendTo(Serializer.Serialize(localPlayer.GetWorldCheck().GetValueOrDefault()), serverEndPoint);
                localPlayer.ClearWorldCheckVector();
            }
        }
    }

    public void JoinServer(InputField field)
    {
        IPAddress adress;
        if (!IPAddress.TryParse(field.text, out adress)) return;

        serverEndPoint = new IPEndPoint(adress, 5554);
        thr.Start();
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
        if (thr != null) thr.Abort();
        if (rcv != null) rcv.Abort();
        if (snd != null) snd.Abort();
        if (clientSocket.Connected) clientSocket.Disconnect(false);
        clientSocket.Close();
    }
}
