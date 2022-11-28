using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageMovement : MonoBehaviour
{
    [SerializeField] private Vector3 endPosition = new Vector3(0.0f,0.0f);
    [SerializeField] private float interval = 0.0f;
    [SerializeField] private float delayTime = 0.0f;

    [Header("Keep same:")]
    [SerializeField] private bool x = false;
    [SerializeField] private bool y = false;
    [SerializeField] private bool z = false;

    private Vector3 startPosition = new Vector3(0.0f, 0.0f);

    void Start()
    {
        if (x && y && z) return;
        if (x) endPosition.x = transform.position.x;
        if (y) endPosition.y = transform.position.y;
        if (z) endPosition.z = transform.position.z;
        startPosition = transform.position;

        StartCoroutine(MoveImage());
    }

    public IEnumerator MoveImage()
    {
        yield return new WaitForSeconds(delayTime);
        float elapsedTime = 0.0f;
        while (elapsedTime < interval)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0.0f, 1.0f, elapsedTime));
            elapsedTime += Time.deltaTime / interval;
            yield return null;
        }
        transform.position = endPosition;
    }
}
