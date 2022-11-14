using UnityEngine;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using System.Net;
using System.Threading;
using UnityEngine.UI;
using System.Text;

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
    PlayerMovement player = new PlayerMovement();

    enum ClientState
    {
        IDLE,
        RECIEVING,
        SENDING,
    }

    ClientState status;

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

        status = ClientState.IDLE;
    }

    private void Update()
    {
        if (connectToScene) ChangeScene();
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
        byte[] recvBuff = new byte[1024];
        clientSocket.ReceiveFrom(recvBuff, ref ep);
        string dataRecv = Encoding.ASCII.GetString(recvBuff);
        Debug.Log("Received message from: " + ep.ToString());
        Debug.Log("Message: " + dataRecv);

    }

    private void Send()
    {
        byte[] sendBuff = new byte[1024];
        string dataSent = "Sup bro";
        sendBuff = Encoding.ASCII.GetBytes(dataSent);

        while (true)
        {
            if (player.stream == null) return;

            else
            {

                clientSocket.SendTo(player.stream.GetBuffer(), ep);
                Debug.Log("Message sent: " + player.stream.ToString());
            }
            
        }
                   

    }

    public void JoinServer()
    {
        ep = new IPEndPoint(IPAddress.Parse(serverIP), 5554);
        thr = new Thread(new ThreadStart(ConnectClient));
        rcv = new Thread(new ThreadStart(Receive));
        snd = new Thread(new ThreadStart(Send));
        thr.Start();
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(1);
        connectToScene = false;
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
