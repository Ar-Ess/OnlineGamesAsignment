﻿using System.Collections;
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
    private uint prevNumLobbyPlayers;

    void Start()
    {
        Instantiate(localPlayer);
        DontDestroyOnLoad(localPlayer);

        GameObject obj = GameObject.Find("UDPServer");
        if (obj != null)
        {
            serverPlayersInfo = obj.GetComponent<UDPServer>();
            maxPlayersText.text = "/ " + serverPlayersInfo.MaxLobbyPlayers.ToString();
            server = true;
            return;
        }

        obj = GameObject.Find("UDPClient");
        if (obj != null)
        {
            clientPlayersInfo = obj.GetComponent<UDPClient>();
            maxPlayersText.text = "/ " + clientPlayersInfo.MaxLobbyPlayers.ToString();
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
