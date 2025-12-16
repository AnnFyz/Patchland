using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSetup : MonoBehaviour
{
    private Transform centerOfGrid;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] CinemachineFramingTransposer transposer;
    [SerializeField] CinemachineComposer composer;
    [SerializeField] float sensitivityZooming = 100f;
    [SerializeField] float sensitivityRotation = 1f;
    [SerializeField] float minDist = 70;
    [SerializeField] float maxDist = 210;
    [SerializeField] float newDist;
    [SerializeField] float offsetX;
    [SerializeField] float offsetY;
    [SerializeField] Transform rotator;
    Vector3 newRotation;
    private void Start()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
        newDist = (minDist + maxDist) / 2;
        transposer.m_CameraDistance = newDist;
        rotator = GameObject.FindGameObjectWithTag("Rotator").transform;
        transform.localRotation = rotator.localRotation;
        StartCoroutine(StartCameraSetup());
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            newDist -= Input.GetAxis("Mouse ScrollWheel") * sensitivityZooming;
            newDist = Mathf.Clamp(newDist, minDist, maxDist);
            transposer.m_CameraDistance = newDist;
        }

        //transform.localRotation = rotator.localRotation;
        newRotation = rotator.localRotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(-newRotation.x, -newRotation.y, 0);

    }

    void Setup()
    {
        centerOfGrid = GridOfPrefabs.Instance.GetCenterObjInGrid();
        virtualCamera.Follow = centerOfGrid;
        //virtualCamera.LookAt = target;
    }

    IEnumerator StartCameraSetup()
    {
        yield return new WaitForSeconds(0.25f);
        Setup();
    }

    IEnumerator SmoothZoom(float targetDistance, float duration)
    {
        float startDistance = transposer.m_CameraDistance;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transposer.m_CameraDistance = Mathf.Lerp(startDistance, targetDistance, elapsed / duration);
            yield return null;
        }
    }
}
