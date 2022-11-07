using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerConnectSceneLogic : MonoBehaviour
{
    public void ChangeScene(string scene)
    {
        SceneManagement.ChangeScene(scene);
    }
}
