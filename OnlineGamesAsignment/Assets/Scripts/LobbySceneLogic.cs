using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneLogic : MonoBehaviour
{
    [SerializeField] private GameObject localPlayer;
    [SerializeField] private Text numPlayersText;
    [SerializeField] private Text maxPlayersText;
    [SerializeField] private GameObject joinText;
    private GameObject obj;

    // Private
    private UDPServer playersInfo = null;
    private int nPlayers;

    void Start()
    {
        Instantiate(localPlayer);
        GameObject obj = GameObject.Find("UDPServer");
        if (obj != null)
        {
            playersInfo = GameObject.Find("UDPServer").GetComponent<UDPServer>();
            int maxPlayers = 0;
            playersInfo.GetPlayersInfo(ref nPlayers, ref maxPlayers);
            maxPlayersText.text = "/ " + maxPlayers.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (playersInfo == null) return;

        int numPlayers = 0, maxPlayers = 0;
        playersInfo.GetPlayersInfo(ref numPlayers, ref maxPlayers);
        if (Input.GetKey(KeyCode.G)) numPlayers++;
        if (numPlayers != nPlayers)
        {
            joinText.SetActive((numPlayers == maxPlayers));
            nPlayers = numPlayers;
            numPlayersText.text = nPlayers.ToString();
        }
    }
}
