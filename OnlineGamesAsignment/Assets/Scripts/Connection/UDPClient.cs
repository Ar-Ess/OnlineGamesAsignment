using UnityEngine;
using System.Net.Sockets;
using System.Net;
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
    public uint NumLobbyPlayers { get { return 2; } }

    // Private
    private Socket clientSocket;
    private Thread cnct, snd, rcv;
    private uint maxLobbyPlayers = 1;
    private PlayerMovement localPlayer = null;
    private OnlinePlayer player = null;
    private UDPClient _instance;
    private EndPoint serverEndPoint = null;
    // BuildPlayer (0) | JoinServer (1) | CreateNewPlayer (2) | GoNextLevel(3) | ThreadStarted(4)
    private StreamFlag callbacks = new StreamFlag(0);
    private uint currentLevel = 0;
    private int threadHealth = -1;

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(this.gameObject);
        else _instance = this;
        DontDestroyOnLoad(this);

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        rcv = new Thread(new ThreadStart(Receive));
        snd = new Thread(new ThreadStart(Send));
    }

    private void Update()
    {
        UpdateHealthLevels(); // Updatejar fora the thread les healths
        AddPlayerLogic(); // Ha d'estar justament aquí. A sobre de tot. No moure.

        JoinServerLogic();
        LookForLocalPlayerInstance(); //TODO: Only in lobby
        BuildPlayers();
        GoNextLevel();
    }

    private void UpdateHealthLevels()
    {
        if (threadHealth != -1)
        {
            player.health = threadHealth;
            threadHealth = -1;
        }
    }

    private void GoNextLevel()
    {
        if (!callbacks.Get(3)) return;
        callbacks.Set(3, false);
        currentLevel++;
        SceneManagement.ChangeScene("Level" + currentLevel.ToString());
    }

    private void AddPlayerLogic()
    {
        if (!callbacks.Get(2)) return;
        callbacks.Set(2, false);

        player = new OnlinePlayer((IPEndPoint)serverEndPoint, false);
        callbacks.Set(0, true);
    }

    private void JoinServerLogic()
    {
        if (!callbacks.Get(1)) return;
        callbacks.Set(1, false);

        SceneManagement.ChangeScene("LobbyScene");
        AddNewPlayer();
    }

    private void LookForLocalPlayerInstance()
    {
        if (localPlayer != null) return;

        GameObject obj = GameObject.FindGameObjectWithTag("LocalPlayer");
        if (obj != null) localPlayer = obj.GetComponentInChildren<PlayerMovement>();
    }

    private void BuildPlayers()
    {
        if (!callbacks.Get(0)) return;
        callbacks.Set(0, false);


        if (!player.built)
        {
            player.BuildOnlinePlayer(Instantiate(onlinePlayer));
            DontDestroyOnLoad(player.player);
        }

    }

    private void ConnectClient()
    {
        clientSocket.Connect(serverEndPoint);
        if (!clientSocket.Connected) return;

        if (!SendData(Serializer.Serialize(DataType.JOIN_REQUEST)))
        {
            clientSocket.Disconnect(true);
            return;
        }

        if (callbacks.Get(4)) return;
        callbacks.Set(4, true);

        snd.Start();
        rcv.Start();
    }

    private void Receive()
    {
        while (true)
        {
            byte[] buffer = new byte[1024];
            clientSocket.ReceiveFrom(buffer, ref serverEndPoint);
            MemoryStream recvStream = new MemoryStream(buffer);
 
            DataType type = Serializer.CheckDataType(recvStream);
            switch (type)
            {
                case DataType.INPUT_FLAG:
                    if (!player.built) break;
                    player.movement.SetFlag(Serializer.Deserialize(recvStream).Uint());
                    break;
                case DataType.WORLD_CHECK:
                    if (!player.built) break;
                    player.movement.SetPosition(Serializer.Deserialize(recvStream).Vector2());
                    break;
                case DataType.LOBBY_MAX:
                    if (!player.built) break;
                    maxLobbyPlayers = Serializer.Deserialize(recvStream).Uint();
                    break;
                case DataType.NEXT_LEVEL:
                    if (!player.built) break;
                    callbacks.Set(3, true);
                    break;
                case DataType.ACCEPT_REQUEST:
                    callbacks.Set(1, true);
                    break;
                case DataType.PLAYER_HEALTH:
                    threadHealth = Serializer.Deserialize(recvStream).Int();
                    break;
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

            if (localPlayer.IsAnyInputActive)
            {
                SendData(Serializer.Serialize(localPlayer.Flag, DataType.INPUT_FLAG));
                localPlayer.ClearInputFlag();
            }

            if (localPlayer.IsSendWorldCheck)
            {
                SendData(Serializer.Serialize(localPlayer.WorldCheck));
                localPlayer.ClearWorldCheckVector();
            }

            if (localPlayer.NeedUpdateHealth)
            {
                SendData(Serializer.Serialize((uint)localPlayer.health, DataType.PLAYER_HEALTH));
                localPlayer.NeedUpdateHealth = false;
            }
        }
    }

    public void JoinServer(InputField field)
    {
        IPAddress adress;
        if (!IPAddress.TryParse(field.text, out adress)) return;

        serverEndPoint = new IPEndPoint(adress, 5554);

        if(!clientSocket.Connected)
        {
            cnct = new Thread(new ThreadStart(ConnectClient));
            cnct.Start();
        }
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

    private bool SendData(byte[] data)
    {
        bool ret = true;
        try
        {
            clientSocket.SendTo(data, serverEndPoint);
        }
        catch
        {
            ret = false;
        }

        return ret;
    }

}
