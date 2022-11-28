using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ClientConnectSceneLogic : MonoBehaviour
{
    [SerializeField] GameObject toMoveOne = null;
    [SerializeField] GameObject toMoveTwo = null;
    private Vector3 startPositionOne = new Vector3(0.0f, 0.0f);
    private Vector3 startPositionTwo = new Vector3(0.0f, 0.0f);
    [SerializeField] private Vector3 endPositionOne = new Vector3(0.0f, 0.0f);
    [SerializeField] private Vector3 endPositionTwo = new Vector3(0.0f, 0.0f);
    [Header("Keep same:")]
    [SerializeField] private bool x = false;
    [SerializeField] private bool y = false;
    [SerializeField] private bool z = false;
    [SerializeField] private float interval = 0.0f;
    [SerializeField] private float delayTime = 0.0f;


    [Serializable]
    public struct ActivableObject
    {
        public GameObject obj;
        public bool active;
        public void SetActive() { obj.SetActive(active); }
    }

    [SerializeField] private ActivableObject[] activables = new ActivableObject[1];
    public void ChangeScene(string scene)
    {
        if (x)
        {
            endPositionOne.x = toMoveOne.transform.position.x;
            endPositionTwo.x = toMoveTwo.transform.position.x;
        }
        if (y)
        {
            endPositionOne.y = toMoveOne.transform.position.y;
            endPositionTwo.y = toMoveTwo.transform.position.y;
        }
        if (z)
        {
            endPositionOne.z = toMoveOne.transform.position.z;
            endPositionTwo.z = toMoveTwo.transform.position.z;
        }
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
