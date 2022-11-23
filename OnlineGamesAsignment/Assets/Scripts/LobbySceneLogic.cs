using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneLogic : MonoBehaviour
{
    [SerializeField] private GameObject localPlayer = null;
    [SerializeField] private Text numPlayersText = null;
    [SerializeField] private Text maxPlayersText = null;
    [SerializeField] private GameObject joinText = null;
    private bool server = false;

    // Private
    private UDPServer serverPlayersInfo = null;
    private UDPClient clientPlayersInfo = null;
    private uint prevNumLobbyPlayers;
    private uint prevMaxLobbyPlayers;

    void Start()
    {
        Instantiate(localPlayer);
        DontDestroyOnLoad(localPlayer);

        GameObject obj = GameObject.Find("UDPServer");
        if (obj != null)
        {
            serverPlayersInfo = obj.GetComponent<UDPServer>();
            maxPlayersText.text = "/ " + serverPlayersInfo.MaxLobbyPlayers.ToString();
            prevNumLobbyPlayers = serverPlayersInfo.NumLobbyPlayers;
            server = true;
            return;
        }

        obj = GameObject.Find("UDPClient");
        if (obj != null)
        {
            clientPlayersInfo = obj.GetComponent<UDPClient>();
            maxPlayersText.text = "/ " + clientPlayersInfo.MaxLobbyPlayers.ToString();
            prevMaxLobbyPlayers = clientPlayersInfo.MaxLobbyPlayers;
            prevNumLobbyPlayers = clientPlayersInfo.NumLobbyPlayers;
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
        uint maxLobbyPlayers = serverPlayersInfo.MaxLobbyPlayers;
        uint numLobbyPlayers = serverPlayersInfo.NumLobbyPlayers;

        UpdateStartGame(numLobbyPlayers, maxLobbyPlayers);
        UpdateUI(numLobbyPlayers, maxLobbyPlayers);
    }

    private void UpdateClient()
    {
        if (clientPlayersInfo == null) return;
        uint maxLobbyPlayers = clientPlayersInfo.MaxLobbyPlayers;
        uint numLobbyPlayers = clientPlayersInfo.NumLobbyPlayers;

        if (maxLobbyPlayers != prevMaxLobbyPlayers)
        {
            maxPlayersText.text = "/ " + maxLobbyPlayers.ToString();
            prevMaxLobbyPlayers = maxLobbyPlayers;
        }

        UpdateUI(numLobbyPlayers, maxLobbyPlayers);
    }

    private void UpdateStartGame(uint numLobbyPlayers, uint maxLobbyPlayers)
    {
        if (numLobbyPlayers == maxLobbyPlayers && Input.GetKeyDown(KeyCode.Return)) 
            SceneManagement.ChangeScene("Level1");
    }

    private void UpdateUI(uint numLobbyPlayers, uint maxLobbyPlayers)
    {
        if (numLobbyPlayers != prevNumLobbyPlayers)
        {
            joinText.SetActive((numLobbyPlayers == maxLobbyPlayers));
            prevNumLobbyPlayers = numLobbyPlayers;
            numPlayersText.text = numLobbyPlayers.ToString();
        }
    }
}
