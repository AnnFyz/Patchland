using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSetup : MonoBehaviour
{
    [SerializeField] Transform target;
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
    }
    private void FixedUpdate()
    {
        Setup();
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
        transform.localRotation = Quaternion.Euler(-newRotation.x,-newRotation.y, 0);

    }

    void Setup()
    {
        if (target == null)
        {
            target = GridOfPrefabs.Instance.GetCenterObjInGrid();
            virtualCamera.Follow = target;
            //virtualCamera.LookAt = target;
        }
    }
}
