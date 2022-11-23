using UnityEngine;
using System.Net;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;
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
    // BuildPlayer (0) |
    private StreamFlag callbacks = new StreamFlag(0);

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
                player.SetOnlinePlayer(Instantiate(onlinePlayer));
                SendData(Serializer.Serialize(maxLobbyPlayers, DataType.LOBBY_MAX), player.ep, 1);
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

        ChangeScene("LobbyScene");
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

            if (localPlayer.IsAnyInputActive())
            {
                foreach (OnlinePlayer player in clients)
                {
                    if (!player.built) continue;
                    SendData(Serializer.Serialize(localPlayer.GetFlag(), DataType.INPUT_FLAG), player.ep, 0);
                }

                localPlayer.ClearFlag();
            }

            if (localPlayer.IsSendingWorldCheck() && localPlayer.GetWorldCheck() != null)
            {
                foreach (OnlinePlayer player in clients)
                {
                    if (!player.built) continue;
                    SendData(Serializer.Serialize(localPlayer.GetWorldCheck().GetValueOrDefault()), player.ep, 0);
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

    private void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    // Only use delay when you call this from the main thread
    private void SendData(byte[] data, EndPoint point, float delay = 0)
    {
        if (delay > 0) 
            SendData_Internal(data, point, delay);
        else
            StartCoroutine(SendData_Internal(data, point, delay));
    }

    private IEnumerator SendData_Internal(byte[] data, EndPoint point, float delay)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        serverSocket.SendTo(data, point);
    }

}
