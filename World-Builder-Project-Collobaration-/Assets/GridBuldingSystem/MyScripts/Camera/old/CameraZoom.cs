using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] float sensitivity = 10f;
    CinemachineComponentBase componentBase;
    CinemachineTransposer cineTransposer;
    float cameraDistance;
    Vector3 startOffset;

    private void Awake()
    {
        cineTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        startOffset = cineTransposer.m_FollowOffset;
    }
    private void Update()
    {
        if(componentBase == null)
        {
            componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);

        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            cineTransposer.m_FollowOffset.y += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            
        }
    }
}
