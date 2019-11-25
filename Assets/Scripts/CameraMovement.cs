using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float startingSize = 3f;
    public float finalCamSize = 20f;

    public float zoomSpeed = 2f;
    public float initalDelay = 0f;

    public Transform toFollow;
    public float followSpeed = .05f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    void Start()
    {
        StartCoroutine(ZoomOut());
    }

    private void Update()
    {
        if (toFollow == null) return;
        var trgPos = new Vector3(toFollow.position.x, toFollow.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, trgPos, followSpeed);
    }

    private IEnumerator ZoomOut()
    {
        cam.orthographicSize = startingSize;
        yield return new WaitForSeconds(initalDelay);
        while (cam.orthographicSize < finalCamSize)
        {
            cam.orthographicSize += zoomSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
