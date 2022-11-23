using UnityEngine;
using System.Net;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class UDPServer : MonoBehaviour
{
    [SerializeField] private GameObject onlinePlayer = null;
    public UDPServer Instance { get { return _instance; } }
    public uint MaxLobbyPlayers { get { return maxLobbyPlayers; } }
    public uint NumLobbyPlayers { get { return (uint)clients.Count + 1; } }

    // Private
    private Socket serverSocket;
    private Thread lstn, snd, rcv;
    private uint maxLobbyPlayers = 1;
    private PlayerMovement localPlayer = null;
    private List<OnlinePlayer> clients = new List<OnlinePlayer>();
    private UDPServer _instance;
    // BuildPlayer (0) | SendInitialData(1) | GoNextLevel(2)
    private StreamFlag callbacks = new StreamFlag(0);
    private uint currentLevel = 0;

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(this.gameObject);
        else _instance = this;
        DontDestroyOnLoad(this);

        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        lstn = new Thread(new ThreadStart(Listen));
        snd = new Thread(new ThreadStart(Send));
        rcv = new Thread(new ThreadStart(Receive));
    }

    private void Update()
    {
        BuildPlayers();
        LookForLocalPlayerInstance();
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
        
        foreach (OnlinePlayer player in clients)
        {
            if (!player.built)
            {
                player.BuildOnlinePlayer(Instantiate(onlinePlayer));
                DontDestroyOnLoad(player.player);
                StartCoroutine(DelayCallback(1, true, 0.3f)); // Do not touch the 0.3
            }
        }
    }

    public void SetMaxLobbyPlayers(Dropdown drop)
    {
        maxLobbyPlayers = (uint)drop.value + 2;
    }

    public void SetupServer()
    {
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5554));

        SceneManagement.ChangeScene("LobbyScene");
        lstn.Start();
    }

    private void Listen()
    {
        int numPlayers = 1;

        while (numPlayers < maxLobbyPlayers)
        {
            EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = new byte[1024];
            int received = serverSocket.ReceiveFrom(data, ref clientEndPoint);
            if (received == 0) continue;
            numPlayers++;
            AddNewClient(clientEndPoint);

            if (numPlayers > 2) continue;
            rcv.Start();
            snd.Start();
        }
    }

    private void AddNewClient(EndPoint point)
    {
        clients.Add(new OnlinePlayer((IPEndPoint)point));
        callbacks.Set(0, true);
    }

    private void Send()
    {
        while (true)
        {
            if (!localPlayer) continue;

            // Player to inform
            if (callbacks.Get(1))
            {
                foreach (OnlinePlayer player in clients)
                {
                    if (player.informed) continue;
                    SendData(Serializer.Serialize(maxLobbyPlayers, DataType.LOBBY_MAX), player.ep);
                    player.informed = true;
                }
                callbacks.Set(1, false);
            }

            // Send player to change level
            if (callbacks.Get(2))
            {
                foreach (OnlinePlayer player in clients)
                    SendData(Serializer.Serialize(DataType.NEXT_LEVEL), player.ep);

                callbacks.Set(2, false);
            }

            if (localPlayer.IsAnyInputActive())
            {
                foreach (OnlinePlayer player in clients)
                {
                    if (!player.built) continue;
                    SendData(Serializer.Serialize(localPlayer.GetFlag(), DataType.INPUT_FLAG), player.ep);
                }

                localPlayer.ClearFlag();
            }

            if (localPlayer.IsSendingWorldCheck() && localPlayer.GetWorldCheck() != null)
            {
                foreach (OnlinePlayer player in clients)
                {
                    if (!player.built) continue;
                    SendData(Serializer.Serialize(localPlayer.GetWorldCheck().GetValueOrDefault()), player.ep);
                }
                localPlayer.ClearWorldCheckVector();
            }
        }
    }

    private void Receive()
    {
        while(true)
        {
            foreach(OnlinePlayer player in clients)
            {
                if (!player.built) continue;

                EndPoint ip = player.ep;
                byte[] buffer = new byte[1024];
                serverSocket.ReceiveFrom(buffer, ref ip);
                MemoryStream recvStream = new MemoryStream(buffer);

                switch (Serializer.CheckDataType(recvStream))
                {
                    case DataType.INPUT_FLAG:
                        player.movement.SetFlag(Serializer.Deserialize(recvStream).Uint());
                        break;
                    case DataType.WORLD_CHECK:
                        player.movement.SetPosition(Serializer.Deserialize(recvStream).Vector2());
                        break;
                }
                recvStream.Flush();
                recvStream.Dispose();
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (lstn != null) lstn.Abort();
        if (snd != null) snd.Abort();
        if (rcv != null) rcv.Abort();
        if (serverSocket.Connected) serverSocket.Disconnect(false);
        serverSocket.Close();
    }

    // Only use delay when you call this from the main thread
    private void SendData(byte[] data, EndPoint point)
    {
        serverSocket.SendTo(data, point);
    }

    private IEnumerator DelayCallback(uint index, bool set, float delay)
    {
        yield return new WaitForSeconds(delay);
        callbacks.Set((ushort)index, set);
    }

    public void GoNextLevel()
    {
        currentLevel++;
        callbacks.Set(2, true);
        SceneManagement.ChangeScene("Level" + currentLevel.ToString());
    }

}
