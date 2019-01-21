using UnityEngine;
using System.Collections.Generic;

// Simple script to allow WASD camera controls with mouse look
public class CamController : MonoBehaviour
{
    public Camera MainCamera = null;
    public Camera[] PortalCameras = null;
    public float LookSpeed = 1.0f;
    public float MoveSpeed = 1.0f;
    public Vector3 PreviousPosition { get; private set; }

    private int CameraIndex = 0;
    private List<Camera> AllCameras;

    private void Start()
    {
        AllCameras = new List<Camera>();
        AllCameras.Add(MainCamera);
        foreach (var cam in PortalCameras)
        {
            AllCameras.Add(cam);
        }
    }

    private void Update()
    {
        // Cycle cameras (to view the scene from the Portal cameras perspective)
        if (Input.GetKeyDown(KeyCode.C))
        {
            AllCameras[CameraIndex].enabled = false;
            CameraIndex = (CameraIndex + 1) % AllCameras.Count;
            AllCameras[CameraIndex].enabled = true;
        }

        // Simple Mouse Look
        if (Input.GetMouseButton(1))
        {
            float xAxis = Input.GetAxis("Mouse X");
            float yAxis = Input.GetAxis("Mouse Y");

            Quaternion yRot = Quaternion.AngleAxis(xAxis * LookSpeed * Time.deltaTime, Vector3.up);
            Quaternion xRot = Quaternion.AngleAxis(-yAxis * LookSpeed * Time.deltaTime, transform.right);

            transform.localRotation = yRot * xRot * transform.localRotation;
        }

        Vector3 displacement = Vector3.zero;

        // Basic Movement
        if (Input.GetKey(KeyCode.W))
        {
            displacement = transform.forward * MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            displacement = -transform.forward * MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            displacement = -transform.right * MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            displacement = transform.right * MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E))
        {
            displacement = Vector3.up * MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            displacement = -Vector3.up * MoveSpeed * Time.deltaTime;
        }

        PreviousPosition = transform.position;
        transform.position += displacement;
    }
}