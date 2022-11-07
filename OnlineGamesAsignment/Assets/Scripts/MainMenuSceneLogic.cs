using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneLogic : MonoBehaviour
{
    public void ChangeScene(string scene)
    {
        SceneManagement.ChangeScene(scene);
    }
}
