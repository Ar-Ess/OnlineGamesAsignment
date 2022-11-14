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
        if (getPlayer && localPlayer == null)
        {
            localPlayer = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<PlayerMovement>();
            getPlayer = false;
        }
        if (connectToScene) ChangeScene("LobbyScene");
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
        byte[] buffer = new byte[1024];
        clientSocket.ReceiveFrom(buffer, ref ep);
        recvStream = new MemoryStream(buffer);
        uint recUint = serializer.Deserialize(recvStream);
        //Debug.Log("Received message from: " + remote.ToString());
        Debug.Log("Message: " + recUint);
        players[0].movement.SetFlag(recUint);
    }

    private void Send()
    {
        while (true)
        {
            if (!localPlayer) continue;
            if (!localPlayer.IsAnyInputActive()) continue;

            clientSocket.SendTo(serializer.Serialize(localPlayer.GetFlag()).GetBuffer(), ep);
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
        if (scene == "LobbyScene")
        {
            connectToScene = false;
            getPlayer = true;
            players.Add(new OnlinePlayer((IPEndPoint)ep, Instantiate(onlinePlayer)));
        }
        SceneManager.LoadScene(scene);
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
