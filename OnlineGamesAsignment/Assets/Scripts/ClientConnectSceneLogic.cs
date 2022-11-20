using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ClientConnectSceneLogic : MonoBehaviour
{
    [SerializeField] GameObject toMoveOne;
    [SerializeField] GameObject toMoveTwo;
    private Vector3 startPositionOne = new Vector3(0.0f, 0.0f);
    private Vector3 startPositionTwo = new Vector3(0.0f, 0.0f);
    [SerializeField] private Vector3 endPositionOne = new Vector3(0.0f, 0.0f);
    [SerializeField] private Vector3 endPositionTwo = new Vector3(0.0f, 0.0f);
    [SerializeField] private float interval = 0.0f;
    [SerializeField] private float delayTime = 0.0f;


    [Serializable]
    public struct ActivableObject
    {
        public GameObject obj;
        public bool active;
        public void SetActive() { obj.SetActive(active); }
    }

    [SerializeField] private ActivableObject[] activables;
    public void ChangeScene(string scene)
    {
        startPositionOne = toMoveOne.transform.position;
        startPositionTwo = toMoveTwo.transform.position;
        StartCoroutine(TransitionBackgroundToScene(scene));
    }

    public IEnumerator TransitionBackgroundToScene(string scene)
    {
        yield return new WaitForSeconds(delayTime);
        foreach (ActivableObject act in activables) act.SetActive();
        float elapsedTime = 0.0f;
        while (elapsedTime < interval)
        {
            toMoveOne.transform.position = Vector3.Lerp(startPositionOne, endPositionOne, Mathf.SmoothStep(0.0f, 1.0f, elapsedTime));
            toMoveTwo.transform.position = Vector3.Lerp(startPositionTwo, endPositionTwo, Mathf.SmoothStep(0.0f, 1.0f, elapsedTime));
            elapsedTime += Time.deltaTime / interval;
            yield return null;
        }
        toMoveOne.transform.position = endPositionOne;
        toMoveTwo.transform.position = endPositionTwo;
        SceneManagement.ChangeScene(scene);
    }
}
