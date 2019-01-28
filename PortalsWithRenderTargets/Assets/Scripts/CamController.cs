using UnityEngine;
using System.Collections.Generic;

// Camera Controller to wrap both PC and Mobile input
public class CamController : MonoBehaviour
{
    public Camera MainCamera;
    public Camera[] PortalCameras;
    public DesktopCamera DesktopCamera;
    public MobileCamera MobileCamera;
    public Vector3 PreviousPosition { get; private set; }

    private int CameraIndex = 0;
    private List<Camera> AllCameras;

    private void Start()
    {
        // Initialize cameras to cycle between
        AllCameras = new List<Camera>();
        AllCameras.Add(MainCamera);
        foreach (var portalCamera in PortalCameras)
        {
            AllCameras.Add(portalCamera);
        }
    }

    private (Quaternion Rotation, Vector3 Displacement) CalculateNextTransform()
    {
        switch (SystemInfo.deviceType)
        {
            case DeviceType.Desktop:
                return DesktopCamera.CalculateNextTransform(
                    transform.right, transform.up, transform.forward);
            case DeviceType.Handheld:
                return MobileCamera.CalculateNextTransform(
                    transform.right, transform.up, transform.forward);
            default:
                return (Rotation: Quaternion.identity, Displacement: Vector3.zero);
        }
    }

    private bool CycleCamera()
    {
        switch (SystemInfo.deviceType)
        {
            case DeviceType.Desktop:
                return DesktopCamera.CycleCamera();
            case DeviceType.Handheld:
                return MobileCamera.CycleCamera();
            default:
                return false;
        }
    }

    private void Update()
    {
        // Cycle cameras (to view the scene from the Portal cameras perspective)
        if (CycleCamera())
        {
            AllCameras[CameraIndex].enabled = false;
            CameraIndex = (CameraIndex + 1) % AllCameras.Count;
            AllCameras[CameraIndex].enabled = true;
        }

        PreviousPosition = transform.position;
        var (Rotation, Displacement) = CalculateNextTransform();
        transform.position += Displacement;
        transform.localRotation = Rotation * transform.localRotation;
    }
}