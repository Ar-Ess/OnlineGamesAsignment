using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerConnectSceneLogic : MonoBehaviour
{
    [SerializeField] GameObject toMove = null;
    private Vector3 startPosition = new Vector3(0.0f, 0.0f);

    [SerializeField] private Vector3 endPosition = new Vector3(0.0f, 0.0f);
    [SerializeField] private float interval = 0.0f;
    [SerializeField] private float delayTime = 0.0f;
    [Header("Keep same:")]
    [SerializeField] private bool x = false;
    [SerializeField] private bool y = false;
    [SerializeField] private bool z = false;
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
        if (x) endPosition.x = toMove.transform.position.x;
        if (y) endPosition.y = toMove.transform.position.y;
        if (z) endPosition.z = toMove.transform.position.z;
        startPosition = toMove.transform.position;
        StartCoroutine(TransitionBackgroundToScene(scene));
    }

    public IEnumerator TransitionBackgroundToScene(string scene)
    {
        yield return new WaitForSeconds(delayTime);
        foreach (ActivableObject act in activables) act.SetActive();
        float elapsedTime = 0.0f;
        while (elapsedTime < interval)
        {
            toMove.transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0.0f, 1.0f, elapsedTime));
            elapsedTime += Time.deltaTime / interval;
            yield return null;
        }
        toMove.transform.position = endPosition;
        SceneManagement.ChangeScene(scene);
    }
}
