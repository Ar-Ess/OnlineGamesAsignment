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
    Thread rcv;
    public InputField field;
    PlayerMovement player;

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

        if(status == ClientState.RECIEVING)
        {
            rcv.Start();
        }
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
        status = ClientState.RECIEVING;
        thr.Abort();

        
    }

    private void Receive()
    {
        data = new byte[1024];
        clientSocket.ReceiveFrom(data, ref ep);
        stringData = Encoding.ASCII.GetString(data);
        Debug.Log("Received message from: " + ep.ToString());
        Debug.Log("Message: " + stringData);
        status = ClientState.SENDING;
        rcv.Abort();
    }

    public void JoinServer()
    {
        ep = new IPEndPoint(IPAddress.Parse(serverIP), 5554);
        thr = new Thread(new ThreadStart(ConnectClient));
        rcv = new Thread(new ThreadStart(Receive));
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
        if (clientSocket.Connected) clientSocket.Disconnect(false);
        clientSocket.Close();
    }


}
