using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour, ICameraManager
{
    [SerializeField] private Camera camera;
    [field: SerializeField] public Camera UI_Camera { get; private set; }
    public Camera ActiveCamera => _currentCamera;
    public Camera OriginalCamera => camera;

    private Camera _currentCamera;


    private void Awake()
    {
        _currentCamera = camera;
    }

    public void SetActiveCamera(Camera cam)
    {
        _currentCamera = cam;
    }
}
