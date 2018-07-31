using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;
using Cinemachine;

public class CameraController : MonoBehaviour,
    IGameEventListener<Camera_GameEvent>
    {

    private CinemachineVirtualCamera _cinemachineVirtualCamera;

    private void Start()
    {
        
    }

    #region On GameEvent

    public void OnGameEvent(Camera_GameEvent eventType)
    {
        switch (eventType.CameraEventType)
        {
            case CameraEventType.ChangeDistance:
                //_cinemachineVirtualCamera.
                break;

            case CameraEventType.ChangeTarget:
                _cinemachineVirtualCamera.Follow = eventType.CameraTarget;
                break;

            default:
                break;
        }
    }

    #endregion
}
