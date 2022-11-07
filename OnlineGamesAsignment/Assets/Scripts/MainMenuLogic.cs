using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuLogic : MonoBehaviour
{
    public void ChangeScene(string scene)
    {
        Debug.Log(scene);
        switch (scene)
        {
            case "ClientConnectScene":
            case "ServerConnectScene":
                SceneManager.LoadScene(scene);
                break;
        }
    }
}
