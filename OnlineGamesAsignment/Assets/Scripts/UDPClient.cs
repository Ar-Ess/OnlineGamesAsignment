using UnityEngine;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using System.Net;
using System.Threading;
using UnityEngine.UI;

public class UDPClient : MonoBehaviour
{
    private static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    EndPoint ep;
    bool connectToScene = false;
    private string serverIP;
    byte[] data = new byte[1024];
    Thread thr;
    public InputField field;
    PlayerMovement player;

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

        data = player.stream.ToArray();
        clientSocket.Send(data, data.Length, SocketFlags.None);
        clientSocket.ReceiveFrom(data, ref ep);
        Debug.Log("received data from" + ep.ToString());

        connectToScene = true;
    }

    public void JoinServer()
    {
        ep = new IPEndPoint(IPAddress.Parse(serverIP), 5554);
        thr = new Thread(new ThreadStart(ConnectClient));
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
        if (clientSocket.Connected) clientSocket.Disconnect(false);
        clientSocket.Close();
    }


}
