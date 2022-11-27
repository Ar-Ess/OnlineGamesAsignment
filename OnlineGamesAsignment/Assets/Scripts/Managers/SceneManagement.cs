using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneManagement
{
    static public void ChangeScene(string scene)
    {
        switch (scene)
        {
            case "ClientConnectScene":
            case "ServerConnectScene":
            case "MainMenuScene":
            case "Level1":
            default:
                SceneManager.LoadScene(scene);
                break;
        }
    }
}
