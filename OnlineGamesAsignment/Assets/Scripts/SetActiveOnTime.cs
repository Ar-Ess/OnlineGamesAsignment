using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveOnTime : MonoBehaviour
{
    [Serializable]
    public struct ActivableObject
    {
        public GameObject obj;
        public bool active;
        public void SetActive() { obj.SetActive(active); }
    }

    [SerializeField] private float m_DelayTime = 0.0f;
    [SerializeField] private ActivableObject[] activables = new ActivableObject[1];
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetActive());
    }

    private IEnumerator SetActive()
    {
        yield return new WaitForSeconds(m_DelayTime);
        foreach (ActivableObject act in activables) act.SetActive();
    }
}
