using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageMovement : MonoBehaviour
{
    [SerializeField] private Vector3 startPosition = new Vector3(0.0f, 0.0f);
    [SerializeField] private Vector3 endPosition = new Vector3(0.0f,0.0f);
    [SerializeField] private float interval = 0.0f;
    [SerializeField] private float delayTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPosition;

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
