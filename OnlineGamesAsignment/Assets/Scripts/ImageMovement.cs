using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageMovement : MonoBehaviour
{
    [SerializeField] private Vector3 startPosition = new Vector3(0.0f, 0.0f);
    [SerializeField] private Vector3 endPosition = new Vector3(0.0f,0.0f);
    [SerializeField] private float interval = 0.0f;
    private float velX = 0.0f;
    private float velY = 0.0f;
    private float velZ = 0.0f;
    private float distance = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        distance = Vector3.Distance(startPosition, endPosition);
        transform.position = startPosition;

        float distX = endPosition.x - startPosition.x;
        float distY = endPosition.y - startPosition.y;
        float distZ = endPosition.z - startPosition.z;

        velX = distX / interval;
        velY = distY / interval;
        velZ = distZ / interval;

        StartCoroutine(MoveImage());
    }

    private IEnumerator MoveImage()
    {
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
