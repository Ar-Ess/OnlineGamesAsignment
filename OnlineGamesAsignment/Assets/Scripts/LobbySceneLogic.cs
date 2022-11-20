using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneLogic : MonoBehaviour
{
    [SerializeField] private GameObject localPlayer;
    [SerializeField] private Text numPlayersText;
    [SerializeField] private Text maxPlayersText;
    [SerializeField] private GameObject joinText;
    private GameObject obj;
    private bool server = false;

    // Private
    private UDPServer serverPlayersInfo = null;
    private UDPClient clientPlayersInfo = null;
    private int nPlayers;

    void Start()
    {
        Instantiate(localPlayer);
        DontDestroyOnLoad(localPlayer);

        GameObject obj = GameObject.Find("UDPServer");
        if (obj != null)
        {
            serverPlayersInfo = obj.GetComponent<UDPServer>();
            int maxPlayers = 0;
            serverPlayersInfo.GetPlayersInfo(ref nPlayers, ref maxPlayers);
            maxPlayersText.text = "/ " + maxPlayers.ToString();
            server = true;
            return;
        }

        obj = GameObject.Find("UDPClient");
        if (obj != null)
        {
            clientPlayersInfo = obj.GetComponent<UDPClient>();
            int maxPlayers = 0;
            clientPlayersInfo.GetPlayersInfo(ref nPlayers, ref maxPlayers);
            maxPlayersText.text = "/ " + maxPlayers.ToString();
            joinText.GetComponent<Text>().text = "ALL READY, WAITING HOST";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (server) UpdateServer();
        else UpdateClient();
    }

    private void UpdateServer()
    {
        if (serverPlayersInfo == null) return;
        int numPlayers = 0, maxPlayers = 0;
        serverPlayersInfo.GetPlayersInfo(ref numPlayers, ref maxPlayers);

        UpdateStartGame(numPlayers, maxPlayers);
        UpdateUI(numPlayers, maxPlayers);
    }

    private void UpdateClient()
    {
        if (clientPlayersInfo == null) return;
        int numPlayers = 0, maxPlayers = 0;
        clientPlayersInfo.GetPlayersInfo(ref numPlayers, ref maxPlayers);

        UpdateUI(numPlayers, maxPlayers);
    }

    private void UpdateStartGame(int numPlayers, int maxPlayers)
    {
        if (numPlayers == maxPlayers && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManagement.ChangeScene("Level1");
        }
    }

    private void UpdateUI(int numPlayers, int maxPlayers)
    {
        if (numPlayers != nPlayers)
        {
            joinText.SetActive((numPlayers == maxPlayers));
            nPlayers = numPlayers;
            numPlayersText.text = nPlayers.ToString();
        }
    }
}
